// store/slices/uiSlice.ts
// UI state slice for Redux store

import { createSlice, PayloadAction } from '@reduxjs/toolkit';

// State interface
interface UIState {
  selectedTimeRange: string;
  isAddAccountFormVisible: boolean;
  theme: 'light' | 'dark' | 'auto';
  hasEverHadAccounts: boolean; // Track if user has ever had accounts
}

// Initial state
const initialState: UIState = {
  selectedTimeRange: 'all',
  isAddAccountFormVisible: false,
  theme: 'auto',
  hasEverHadAccounts: false,
};

// Slice
const uiSlice = createSlice({
  name: 'ui',
  initialState,
  reducers: {
    setSelectedTimeRange: (state, action: PayloadAction<string>) => {
      state.selectedTimeRange = action.payload;
    },
    setAddAccountFormVisible: (state, action: PayloadAction<boolean>) => {
      state.isAddAccountFormVisible = action.payload;
    },
    setTheme: (state, action: PayloadAction<'light' | 'dark' | 'auto'>) => {
      state.theme = action.payload;
    },
    setHasEverHadAccounts: (state, action: PayloadAction<boolean>) => {
      state.hasEverHadAccounts = action.payload;
    },
  },
});

export const {
  setSelectedTimeRange,
  setAddAccountFormVisible,
  setTheme,
  setHasEverHadAccounts,
} = uiSlice.actions;

export default uiSlice.reducer;
