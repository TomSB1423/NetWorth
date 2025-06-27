import { useColorScheme } from "react-native";
import { useAppDispatch, useTheme } from "../store/hooks";
import { setTheme } from "../store/slices/uiSlice";

export type ColorSchemeMode = "auto" | "light" | "dark";
export type EffectiveColorScheme = "light" | "dark";

export function useColorSchemeToggle(): [EffectiveColorScheme, ColorSchemeMode, (mode: ColorSchemeMode) => void] {
  const systemScheme = useColorScheme();
  const themeMode = useTheme(); // Get theme from Redux
  const dispatch = useAppDispatch();
  
  // Calculate effective color scheme
  const effectiveColorScheme: EffectiveColorScheme = 
    themeMode === "auto"
      ? systemScheme === "dark" ? "dark" : "light"
      : themeMode;
  
  // Create setter that dispatches to Redux
  const setMode = (mode: ColorSchemeMode) => {
    dispatch(setTheme(mode));
  };
  
  return [effectiveColorScheme, themeMode, setMode];
}
