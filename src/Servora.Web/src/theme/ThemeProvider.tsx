import {
  createContext,
  useContext,
  useEffect,
  useMemo,
  useState,
  type ReactNode,
} from 'react';
import { applyTheme } from './applyTheme';
import {
  defaultThemeName,
  themes,
  type ThemeName,
  type ThemeTokens,
} from './tokens';

type ThemeContextValue = {
  themeName: ThemeName;
  theme: ThemeTokens;
  setThemeName: (name: ThemeName) => void;
};

const ThemeContext = createContext<ThemeContextValue | null>(null);

type ThemeProviderProps = {
  children: ReactNode;
  initialTheme?: ThemeName;
};

export function ThemeProvider({
  children,
  initialTheme = defaultThemeName,
}: ThemeProviderProps) {
  const [themeName, setThemeName] = useState<ThemeName>(initialTheme);

  useEffect(() => {
    applyTheme(themes[themeName]);
  }, [themeName]);

  const value = useMemo<ThemeContextValue>(
    () => ({
      themeName,
      theme: themes[themeName],
      setThemeName,
    }),
    [themeName],
  );

  return <ThemeContext.Provider value={value}>{children}</ThemeContext.Provider>;
}

export function useTheme(): ThemeContextValue {
  const context = useContext(ThemeContext);
  if (!context) {
    throw new Error('useTheme must be used within ThemeProvider');
  }
  return context;
}
