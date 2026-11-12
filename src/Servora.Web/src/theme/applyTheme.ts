import type { ThemeTokens } from './tokens';

export function applyTheme(tokens: ThemeTokens): void {
  const root = document.documentElement;

  root.dataset.theme = tokens.name;
  root.style.setProperty('--color-background', tokens.colors.background);
  root.style.setProperty('--color-surface', tokens.colors.surface);
  root.style.setProperty('--color-surface-muted', tokens.colors.surfaceMuted);
  root.style.setProperty('--color-border', tokens.colors.border);
  root.style.setProperty('--color-text', tokens.colors.text);
  root.style.setProperty('--color-text-muted', tokens.colors.textMuted);
  root.style.setProperty('--color-primary', tokens.colors.primary);
  root.style.setProperty('--color-primary-text', tokens.colors.primaryText);
  root.style.setProperty('--color-danger', tokens.colors.danger);
  root.style.setProperty('--color-focus', tokens.colors.focus);
  root.style.setProperty('--radius-sm', tokens.radii.sm);
  root.style.setProperty('--radius-md', tokens.radii.md);
  root.style.setProperty('--radius-lg', tokens.radii.lg);
  root.style.setProperty('--shadow-sm', tokens.shadows.sm);
  root.style.setProperty('--font-body', tokens.fonts.body);
}
