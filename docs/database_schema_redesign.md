# Database Schema Redesign

## Overview

This document summarizes the database schema redesign implemented to align with the GoCardless integration flow and proper separation of concerns.

## Changes Summary

### Removed Tables

- **UserInstitutions** - Removed as user-institution connections are now tracked through Agreements and Requisitions

### New Tables

#### Agreements

Stores user agreements for accessing bank account data through GoCardless.

| Column | Type | Description |
|--------|------|-------------|
| Id | string (PK) | GoCardless agreement ID |
| UserId | string (FK) | User who owns this agreement |
| InstitutionId | string (FK) | Institution metadata ID |
| MaxHistoricalDays | int? | Maximum days of historical data available |
| AccessValidForDays | int? | Number of days the access token is valid |
| AccessScope | string (JSON) | Access scopes granted (e.g., balances, transactions) |
| Reconfirmation | bool | Whether reconfirmation is required |
| Created | DateTime | Agreement creation timestamp |
| Accepted | DateTime? | Agreement acceptance timestamp |

**Relationships:**

- One-to-Many with Requisitions
- Many-to-One with User (cascade delete)
- Many-to-One with InstitutionMetadata (restrict delete)

#### AccountBalances

Stores account balance snapshots at specific points in time.

| Column | Type | Description |
|--------|------|-------------|
| Id | string (PK) | Unique identifier |
| AccountId | string (FK) | Account this balance belongs to |
| BalanceType | string | Type of balance (e.g., interimAvailable, expected) |
| Amount | decimal(18,2) | Balance amount |
| Currency | string(3) | Currency code |
| ReferenceDate | DateTime? | Reference date for this balance |
| RetrievedAt | DateTime | When this balance was retrieved |

**Relationships:**

- Many-to-One with Account (cascade delete)

**Indexes:**

- AccountId
- RetrievedAt
- Composite: (AccountId, BalanceType, RetrievedAt)

### Updated Tables

#### InstitutionMetadata (formerly Institutions cache)

Added supported_features field to track institution capabilities.

| Column | Type | Description |
|--------|------|-------------|
| Id | string (PK composite) | GoCardless institution ID |
| CountryCode | string(2) (PK composite) | Country code |
| Name | string(200) | Institution name |
| LogoUrl | string(500) | Institution logo URL |
| Bic | string(50) | Bank Identifier Code |
| Countries | jsonb | Countries where institution operates |
| **SupportedFeatures** | **jsonb** | **NEW: Supported features (e.g., payments, account_selection)** |
| LastUpdated | DateTime | Cache last updated timestamp |

**New Relationships:**

- One-to-Many with Agreements

#### Requisitions

Updated to include all GoCardless fields and proper relationships.

**New/Changed Fields:**

| Column | Type | Description |
|--------|------|-------------|
| **UserId** | **string(255) (FK)** | **NEW: User who created this requisition** |
| Redirect | string(500) | Redirect URL after completion |
| Status | string(50) | Requisition status (enum) |
| InstitutionId | string(255) (FK) | Institution metadata ID |
| AgreementId | string(255) (FK) | Agreement ID |
| Reference | string(200) | Reference identifier |
| Accounts | jsonb | **CHANGED: Now JSON array (was string array)** |
| **UserLanguage** | **string(10)** | **NEW: User language preference** |
| Link | string(500) | **CHANGED: Renamed from AuthenticationLink** |
| **Ssn** | **string(50)** | **NEW: SSN if required by institution** |
| **AccountSelection** | **bool** | **CHANGED: Now boolean (was string)** |
| **RedirectImmediate** | **bool** | **NEW: Whether immediate redirect is enabled** |
| Created | DateTime | Creation timestamp |

**New Relationships:**

- Many-to-One with User (cascade delete)
- Many-to-One with InstitutionMetadata (restrict delete)
- Many-to-One with Agreement (restrict delete)

**Indexes:**

- UserId
- Status
- InstitutionId
- AgreementId
- Created

#### Accounts

Restructured to reference Requisition and InstitutionMetadata directly, with additional GoCardless fields.

**Before:**

| Column | Type |
|--------|------|
| Id | string(100) |
| OwnerId | string(100) |
| UserInstitutionId | string(100) |
| InstitutionId | string(100) |
| Name | string(200) |

**After:**

| Column | Type | Description |
|--------|------|-------------|
| Id | string(255) | GoCardless account ID |
| **UserId** | **string(255) (FK)** | **CHANGED: Renamed from OwnerId** |
| **RequisitionId** | **string(255) (FK)** | **NEW: Requisition that created this account** |
| InstitutionId | string(255) (FK) | **CHANGED: Now references InstitutionMetadata directly** |
| Name | string(200) | Account name |
| **Iban** | **string(34)** | **NEW: Account IBAN** |
| **Currency** | **string(3)** | **NEW: Account currency** |
| **Product** | **string(200)** | **NEW: Product name/type** |
| **CashAccountType** | **string(50)** | **NEW: Cash account type (e.g., CACC)** |
| **AdditionalAccountData** | **jsonb** | **NEW: Additional account data** |
| **Created** | **DateTime** | **NEW: Account creation timestamp** |
| **LastSynced** | **DateTime?** | **NEW: Last sync timestamp** |

**Removed:**

- UserInstitutionId (FK to removed Institution table)

**New Relationships:**

- Many-to-One with Requisition (restrict delete)
- Many-to-One with InstitutionMetadata (restrict delete)
- One-to-Many with AccountBalances

**Indexes:**

- UserId
- RequisitionId
- InstitutionId
- Created

#### Transactions

Added comprehensive GoCardless transaction fields.

**Before:**

| Column | Type |
|--------|------|
| Id | string(100) |
| OwnerId | string(100) |
| AccountId | string(100) |
| Value | decimal(18,2) |
| Currency | string(3) |
| Time | DateTime |

**After:**

| Column | Type | Description |
|--------|------|-------------|
| Id | string(255) | Unique identifier |
| **UserId** | **string(255) (FK)** | **CHANGED: Renamed from OwnerId** |
| AccountId | string(255) (FK) | Account ID |
| **TransactionId** | **string(255)** | **NEW: GoCardless transaction ID** |
| **DebtorName** | **string(500)** | **NEW: Debtor (payer) name** |
| **DebtorAccountIban** | **string(34)** | **NEW: Debtor account IBAN** |
| **Amount** | **decimal(18,2)** | **CHANGED: Renamed from Value** |
| Currency | string(3) | Currency code |
| **BankTransactionCode** | **string(100)** | **NEW: Bank transaction code** |
| **BookingDate** | **DateTime?** | **NEW: Transaction booking date** |
| **ValueDate** | **DateTime?** | **NEW: Transaction value date** |
| **RemittanceInformationUnstructured** | **string(1000)** | **NEW: Transaction description** |
| **Status** | **string(50)** | **NEW: Transaction status** |
| **ImportedAt** | **DateTime** | **NEW: Import timestamp** |

**Removed:**

- Time (replaced by BookingDate/ValueDate)
- Value (renamed to Amount)

**Indexes:**

- UserId
- AccountId
- TransactionId
- BookingDate
- ValueDate
- ImportedAt

#### Users

Simplified relationships after removing UserInstitutions.

**Changes:**

- Removed UserInstitutions navigation property
- Updated OwnerId references to UserId throughout
- Updated all FK property names from OwnerId → UserId

## Data Flow

### Account Linking Flow

1. User creates **Agreement** with Institution
2. **Requisition** is created referencing the Agreement
3. User authenticates with bank → **Account**(s) created referencing Requisition
4. **Transaction**s and **AccountBalance**s synced for each Account

### Entity Relationships

```
User
├── Agreements (1:N)
│   └── Requisitions (1:N)
│       └── Accounts (via RequisitionId reference)
├── Accounts (1:N)
│   ├── Balances (1:N)
│   └── Transactions (1:N)
└── Transactions (1:N)

InstitutionMetadata
├── Agreements (1:N)
└── Accounts (via InstitutionId reference)

Agreement
└── Requisitions (1:N)
```

## Migration Notes

### Breaking Changes

1. **Institution table removed** - User-institution connections now tracked via Agreements
2. **Account.OwnerId → UserId** - Property renamed throughout codebase
3. **Transaction.OwnerId → UserId** - Property renamed throughout codebase
4. **Transaction.Value → Amount** - Property renamed
5. **Transaction.Time removed** - Use BookingDate/ValueDate instead
6. **Requisition.Accounts** - Changed from string[] to JSON string
7. **Requisition.AuthenticationLink → Link** - Property renamed
8. **Requisition.AccountSelection** - Changed from string to bool

### Required Code Updates

- ✅ Domain entities updated (UserAccount, Transaction)
- ✅ Infrastructure entities updated (Account, Transaction, Requisition)
- ✅ New entities created (Agreement, AccountBalance)
- ✅ EF Core configurations updated
- ✅ Repository mappings updated
- ✅ API response models updated (UserAccountResponse)
- ✅ DbContext updated

### Database Recreation Required

Since the application uses `EnsureCreatedAsync()` instead of migrations, the database will be recreated on next run with the new schema.

**⚠️ Warning**: All existing data will be lost. Ensure this is acceptable for your development environment.

## Future Considerations

1. **Migrations**: Consider implementing EF Core migrations for production deployments
2. **Agreement Repository**: Create IAgreementRepository and implementation
3. **AccountBalance Repository**: Create IAccountBalanceRepository and implementation
4. **Sync Logic**: Update account/transaction sync handlers to populate new fields
5. **Balance Tracking**: Implement balance snapshot creation during account sync
