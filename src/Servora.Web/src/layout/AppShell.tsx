import type { ReactNode } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '@/auth/AuthContext';
import { Sidebar } from '@/layout/Sidebar';
import { useTheme } from '@/theme';

function SunIcon() {
  return (
    <svg
      className="theme-icon"
      viewBox="0 0 24 24"
      fill="none"
      stroke="currentColor"
      strokeWidth="2"
      strokeLinecap="round"
      strokeLinejoin="round"
      aria-hidden="true"
    >
      <circle cx="12" cy="12" r="4" />
      <path d="M12 2v2M12 20v2M4.93 4.93l1.41 1.41M17.66 17.66l1.41 1.41M2 12h2M20 12h2M4.93 19.07l1.41-1.41M17.66 6.34l1.41-1.41" />
    </svg>
  );
}

function MoonIcon() {
  return (
    <svg
      className="theme-icon"
      viewBox="0 0 24 24"
      fill="none"
      stroke="currentColor"
      strokeWidth="2"
      strokeLinecap="round"
      strokeLinejoin="round"
      aria-hidden="true"
    >
      <path d="M21 14.5A8.5 8.5 0 1 1 9.5 3a7 7 0 0 0 11.5 11.5z" />
    </svg>
  );
}

type AppShellProps = {
  children: ReactNode;
  contentClassName?: string;
  showSidebar?: boolean;
};

export function AppShell({
  children,
  contentClassName = 'content',
  showSidebar = false,
}: AppShellProps) {
  const navigate = useNavigate();
  const { isAuthenticated, logout } = useAuth();
  const { themeName, setThemeName } = useTheme();

  return (
    <div className="app-shell">
      <header className="topbar">
        <Link to={isAuthenticated ? '/masa-takip' : '/'} className="topbar-brand">
          Servora
        </Link>
        <nav>
          {isAuthenticated ? (
            <button
              className="btn"
              type="button"
              onClick={() => {
                void logout()
                  .catch(() => undefined)
                  .finally(() => navigate('/'));
              }}
            >
              Çıkış
            </button>
          ) : null}
          <button
            className="btn btn-icon"
            type="button"
            aria-label={
              themeName === 'dark' ? 'Açık temaya geç' : 'Koyu temaya geç'
            }
            title={themeName === 'dark' ? 'Açık temaya geç' : 'Koyu temaya geç'}
            onClick={() =>
              setThemeName(themeName === 'default' ? 'dark' : 'default')
            }
          >
            {themeName === 'dark' ? <SunIcon /> : <MoonIcon />}
          </button>
        </nav>
      </header>

      <div className={`app-body${showSidebar ? ' has-sidebar' : ''}`}>
        {showSidebar ? <Sidebar /> : null}
        <main className={contentClassName}>{children}</main>
      </div>
    </div>
  );
}
