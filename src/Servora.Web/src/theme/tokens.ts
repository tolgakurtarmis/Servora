export type ThemeName = 'default' | 'dark';

export type ThemeTokens = {
  name: ThemeName;
  colors: {
    background: string;
    surface: string;
    surfaceMuted: string;
    border: string;
    text: string;
    textMuted: string;
    primary: string;
    primaryText: string;
    danger: string;
    focus: string;
  };
  radii: {
    sm: string;
    md: string;
    lg: string;
  };
  shadows: {
    sm: string;
  };
  fonts: {
    body: string;
  };
};

export const themes: Record<ThemeName, ThemeTokens> = {
  default: {
    name: 'default',
    colors: {
      background: '#f4f6f8',
      surface: '#ffffff',
      surfaceMuted: '#eef2f6',
      border: '#d7dee7',
      text: '#1f2933',
      textMuted: '#62748a',
      primary: '#2563eb',
      primaryText: '#ffffff',
      danger: '#dc2626',
      focus: '#93c5fd',
    },
    radii: {
      sm: '6px',
      md: '10px',
      lg: '16px',
    },
    shadows: {
      sm: '0 8px 24px rgba(15, 23, 42, 0.08)',
    },
    fonts: {
      body: '"Segoe UI", system-ui, -apple-system, sans-serif',
    },
  },
  dark: {
    name: 'dark',
    colors: {
      background: '#0f172a',
      surface: '#1e293b',
      surfaceMuted: '#334155',
      border: '#475569',
      text: '#f8fafc',
      textMuted: '#94a3b8',
      primary: '#3b82f6',
      primaryText: '#ffffff',
      danger: '#f87171',
      focus: '#60a5fa',
    },
    radii: {
      sm: '6px',
      md: '10px',
      lg: '16px',
    },
    shadows: {
      sm: '0 8px 24px rgba(0, 0, 0, 0.35)',
    },
    fonts: {
      body: '"Segoe UI", system-ui, -apple-system, sans-serif',
    },
  },
};

export const defaultThemeName: ThemeName = 'default';
