# File Structure Review - NetWorth Application

## Overview
This document provides a comprehensive review of the current file structure and recommendations for improvements based on React Native best practices and the project's custom coding instructions.

## Current Structure Analysis

### ✅ Well-Organized Areas

1. **App Router Structure (`/app`)**
   - Follows Expo Router conventions with file-based routing
   - Clear separation of tabs and standalone screens
   - Proper use of layout files

2. **Component Organization (`/components`)**
   - Descriptive component names
   - TypeScript usage throughout
   - Good separation of UI concerns

3. **Constants (`/constants`)**
   - Centralized color palette management
   - Follows project requirement for consistent styling

4. **Services (`/services`)**
   - Good separation of concerns
   - Type definitions included
   - Service provider pattern implementation

5. **Utilities (`/utils`)**
   - Well-organized utility functions
   - Security, validation, and debug utilities

## ❌ Issues Identified

### 1. **Duplicate Context Management** - HIGH PRIORITY
**Problem**: Two separate context directories exist:
- `/contexts/AuthContext.tsx` (plural)
- `/context/AppContext.tsx` (singular)

**Impact**: 
- Confusion for developers
- Inconsistent naming conventions
- Potential for duplicate functionality

**Recommendation**: Consolidate into single `/contexts` directory

### 2. **Mixed State Management Patterns** - HIGH PRIORITY
**Problem**: Using both Redux and React Context:
- Redux store setup in `/store` with slices
- React Context with useReducer in `/context/AppContext.tsx`

**Impact**:
- Violates single source of truth principle
- Increases complexity and bundle size
- Potential for state synchronization issues

**Recommendation**: Choose one approach (prefer Redux for complex state management)

### 3. **Test Configuration Issues** - MEDIUM PRIORITY
**Problem**: Jest tests failing with window property redefinition errors
```
TypeError: Cannot redefine property: window
```

**Impact**: 
- Cannot run automated tests
- No test coverage verification
- Breaks CI/CD pipeline

**Recommendation**: Fix Jest configuration and test environment setup

### 4. **Component Organization** - MEDIUM PRIORITY
**Problem**: All components in flat structure under `/components`

**Current**:
```
/components
  ├── AccountsList.tsx
  ├── BankTransactions.tsx
  ├── CategoryBreakdown.tsx
  ├── DebugMenu.tsx
  ├── ErrorBoundary.tsx
  ├── LoadingSpinner.tsx
  ├── MetricsCard.tsx
  ├── MonthlyFlowChart.tsx
  ├── NetWorthChart.tsx
  ├── SummaryCard.tsx
  └── WelcomeOverlay.tsx
```

**Recommendation**: Organize into logical subcategories

### 5. **Missing Essential Directories** - LOW PRIORITY
**Missing Directories**:
- `/lib` - Third-party library configurations
- `/api` - API client and endpoints
- `/screens` - Screen components (if separating from Expo Router)

## Recommended Structure

```
├── app/                          # Expo Router screens
│   ├── (tabs)/                   # Tab navigation
│   ├── onboarding/               # Onboarding flow
│   └── _layout.tsx               # Root layout
├── components/                   # Reusable UI components
│   ├── ui/                       # Basic UI components
│   │   ├── LoadingSpinner.tsx
│   │   ├── ErrorBoundary.tsx
│   │   └── WelcomeOverlay.tsx
│   ├── charts/                   # Chart components
│   │   ├── NetWorthChart.tsx
│   │   ├── MonthlyFlowChart.tsx
│   │   └── CategoryBreakdown.tsx
│   ├── forms/                    # Form components
│   │   └── AccountsList.tsx
│   ├── cards/                    # Card components
│   │   ├── MetricsCard.tsx
│   │   └── SummaryCard.tsx
│   └── features/                 # Feature-specific components
│       ├── BankTransactions.tsx
│       └── DebugMenu.tsx
├── contexts/                     # React contexts (consolidated)
│   └── AuthContext.tsx
├── store/                        # Redux store (if keeping Redux)
│   ├── slices/
│   ├── hooks.ts
│   └── index.ts
├── services/                     # Business logic and API
│   ├── api/                      # API related services
│   ├── mock/                     # Mock services for development
│   └── types.ts
├── utils/                        # Utility functions
├── constants/                    # App constants
├── hooks/                        # Custom React hooks
├── types/                        # TypeScript type definitions
├── assets/                       # Static assets
├── docs/                         # Documentation
└── __tests__/                    # Test files
```

## Priority Fixes

### Priority 1: Fix State Management Conflict
1. Decide on Redux vs Context API
2. Remove or consolidate duplicate state management
3. Update all imports and dependencies

### Priority 2: Consolidate Context Directories
1. Move `/context/AppContext.tsx` to `/contexts/`
2. Update all imports
3. Remove empty `/context` directory

### Priority 3: Fix Jest Configuration
1. Resolve window property redefinition
2. Update test environment configuration
3. Ensure all tests pass

### Priority 4: Organize Components
1. Create component subdirectories
2. Move components to appropriate categories
3. Update imports throughout the application

## Implementation Guidelines

### Directory Naming Conventions
- Use **plural** names for directories containing multiple items (`components/`, `contexts/`, `services/`)
- Use **lowercase** with hyphens for multi-word directories
- Be consistent across the entire project

### File Naming Conventions
- **PascalCase** for React components (`ComponentName.tsx`)
- **camelCase** for utilities and hooks (`useCustomHook.ts`)
- **kebab-case** for configuration files (`jest.config.json`)

### Import Organization
```typescript
// 1. External libraries
import React from 'react';
import { View, Text } from 'react-native';

// 2. Internal modules (absolute imports preferred)
import { useAuth } from '@/contexts/AuthContext';
import { Button } from '@/components/ui/Button';

// 3. Relative imports
import './ComponentName.styles';
```

## Testing Strategy
1. Fix Jest configuration issues
2. Ensure all components have basic render tests
3. Add integration tests for critical user flows
4. Configure test coverage thresholds

## Migration Steps
1. **Assessment**: Review current dependencies and usage
2. **Planning**: Create detailed migration plan
3. **Implementation**: Make changes incrementally
4. **Testing**: Verify all functionality works after changes
5. **Documentation**: Update this document and other docs

## Conclusion
The current file structure has a solid foundation but needs consolidation and organization improvements. The main issues are duplicate state management patterns and inconsistent directory naming. Addressing these issues will improve maintainability, developer experience, and code clarity.