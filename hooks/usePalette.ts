import { palette } from "../constants/palette";
import { useColorSchemeToggle } from "./useColorSchemeToggle";

/**
 * Custom hook to get the current palette based on the color scheme.
 * Returns the palette for the current color scheme (light or dark).
 */
export function usePalette() {
  const [colorScheme] = useColorSchemeToggle();
  return colorScheme === "dark" ? palette.dark : palette.light;
}
