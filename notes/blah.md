# Networth Application - Landing Page & Dashboard Design

## Design Principles

- Professional, clean aesthetic - no emojis
- Trust-focused financial branding (blues, greens, neutral tones)
- Clear typography hierarchy
- Responsive design (mobile-first)

---

## Landing Page

### Hero Section

**Headline:** "Know Your Net Worth at a Glance"
**Subheadline:** "All your financial accounts in one powerful dashboard. Track your wealth, set goals, and make smarter financial decisions."

**CTA Buttons:**

- Primary: "Get Started" (Sign Up)
- Secondary: "Sign In"

### Features Section

| Feature | Title | Description |
|---------|-------|-------------|
| Bank Sync | Automatic Account Sync | Connect your bank accounts securely via open banking. Transactions update automatically. |
| Dashboard | Unified Dashboard | See all your accounts, balances, and net worth in one place. |
| Trends | Historical Tracking | Visualize your net worth over time with interactive charts. |
| Goals | Financial Goals | Set savings targets and track progress toward your financial milestones. |
| Security | Bank-Grade Security | PSD2-compliant data access. Your credentials are never stored. |
| Multi-Account | Multi-Institution Support | Connect accounts from hundreds of banks and financial institutions. |

### How It Works Section

1. **Connect** - Link your bank accounts securely in seconds
2. **Sync** - We automatically fetch your transactions and balances
3. **Track** - Monitor your net worth and financial health over time

### Trust Indicators

- "Powered by Open Banking"
- "256-bit encryption"
- "Read-only access to your accounts"
- Institution logos (supported banks)

### Footer

- About / Privacy Policy / Terms of Service
- Contact / Support links

---

## Dashboard

### Layout Structure

```
+--------------------------------------------------+
|  Header: Logo | Navigation | User Menu           |
+--------------------------------------------------+
|  Net Worth Summary Card                          |
|  [Total Net Worth: £XX,XXX.XX] [+X.X% this month]|
+--------------------------------------------------+
|  Net Worth Graph (Area Chart)                    |
|  [1W] [1M] [3M] [6M] [1Y] [ALL] time toggles     |
+--------------------------------------------------+
|  Accounts List        |  Accounts Breakdown      |
|  (Sortable table)     |  (Donut/Pie chart)       |
+--------------------------------------------------+
|  Goals Progress                                  |
|  (Progress bars with targets)                    |
+--------------------------------------------------+
```

### Components

#### 1. Net Worth Summary Card

- **Total Net Worth** - Large, prominent figure
- **Change indicator** - Percentage and absolute change (period selectable)
- **Last synced** - Timestamp of most recent data refresh
- **Sync button** - Manual refresh option

#### 2. Net Worth Graph

- Area chart showing net worth over time
- Time range selector: 1 Week, 1 Month, 3 Months, 6 Months, 1 Year, All Time
- Hover tooltip showing exact value and date
- Optional: Toggle to show/hide individual account lines

#### 3. Accounts List

| Column | Description |
|--------|-------------|
| Institution | Bank logo + name |
| Account Name | Account nickname or type |
| Type | Checking / Savings / Credit / Investment |
| Balance | Current balance (color-coded: positive green, negative red) |
| Last Updated | Relative timestamp |
| Actions | Refresh / Remove / Details |

- Sortable by any column
- Expandable rows for transaction preview
- "Add Account" button

#### 4. Accounts Breakdown Chart

- Donut chart showing balance distribution by:
  - Account type (default view)
  - Institution (toggle option)
- Legend with account names and percentages
- Click segment to filter accounts list

#### 5. Goals Section

- List of user-defined financial goals
- Each goal shows:
  - Goal name (e.g., "Emergency Fund", "House Deposit")
  - Target amount
  - Current progress (linked account or manual)
  - Progress bar with percentage
  - Projected completion date
- "Add Goal" button
- Edit/Delete options per goal

### Additional Dashboard Features (Future)

- [ ] Transaction search and filtering
- [ ] Spending categories breakdown
- [ ] Monthly cash flow summary
- [ ] Recurring transaction detection
- [ ] Export data (CSV/PDF)
- [ ] Dark mode toggle

---

## Navigation Structure

```
Landing Page (/)
├── Sign Up (/signup)
├── Sign In (/login)
└── Dashboard (/dashboard) [Protected]
    ├── Accounts (/dashboard/accounts)
    ├── Goals (/dashboard/goals)
    ├── Transactions (/dashboard/transactions)
    └── Settings (/dashboard/settings)
```

---

## Color Palette (Suggested)

| Usage | Color | Hex |
|-------|-------|-----|
| Primary | Deep Blue | #1E3A5F |
| Secondary | Teal | #2A9D8F |
| Success/Positive | Green | #059669 |
| Warning | Amber | #D97706 |
| Error/Negative | Red | #DC2626 |
| Background | Off-white | #F8FAFC |
| Card Background | White | #FFFFFF |
| Text Primary | Dark Gray | #1F2937 |
| Text Secondary | Medium Gray | #6B7280 |
