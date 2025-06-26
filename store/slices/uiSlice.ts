// store/slices/uiSlice.ts
// UI state slice for Redux store

import { createSlice, PayloadAction } from '@reduxjs/toolkit';

// State interface
interface UIState {
  selectedTimeRange: string;
  theme: 'light' | 'dark' | 'auto';
  isFirstLaunch: boolean; // Track if this is the first launch after reset
}

// Initial state
const initialState: UIState = {
  selectedTimeRange: 'all',
  theme: 'auto',
  isFirstLaunch: true, // Default to true for new users
};

// Slice
const uiSlice = createSlice({
  name: 'ui',
  initialState,
  reducers: {
    setSelectedTimeRange: (state, action: PayloadAction<string>) => {
      state.selectedTimeRange = action.payload;
    },
    setTheme: (state, action: PayloadAction<'light' | 'dark' | 'auto'>) => {
      state.theme = action.payload;
    },
    setFirstLaunch: (state, action: PayloadAction<boolean>) => {
      state.isFirstLaunch = action.payload;
    },
  },
});

export const {
  setSelectedTimeRange,
  setTheme,
  setFirstLaunch,
} = uiSlice.actions;

export default uiSlice.reducer;
