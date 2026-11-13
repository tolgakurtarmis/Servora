import { useEffect, useMemo, useState } from 'react';
import { NavLink, useLocation } from 'react-router-dom';
import { useAuth } from '@/auth/AuthContext';
import { filterNavItems, isNavItemActive, navItems } from '@/nav/navItems';
import type { NavItem } from '@/nav/types';

function ChevronIcon({ open }: { open: boolean }) {
  return (
    <svg
      className={`sidebar-chevron${open ? ' is-open' : ''}`}
      viewBox="0 0 24 24"
      fill="none"
      stroke="currentColor"
      strokeWidth="2"
      strokeLinecap="round"
      strokeLinejoin="round"
      aria-hidden="true"
    >
      <path d="M9 6l6 6-6 6" />
    </svg>
  );
}

function NavLeaf({ item }: { item: NavItem }) {
  if (!item.path) {
    return null;
  }

  return (
    <NavLink
      to={item.path}
      className={({ isActive }) =>
        `sidebar-link${isActive ? ' is-active' : ''}`
      }
      end
    >
      {item.label}
    </NavLink>
  );
}

function NavGroup({ item }: { item: NavItem }) {
  const location = useLocation();
  const children = item.children ?? [];
  const hasActiveChild = isNavItemActive(item, location.pathname);
  const [open, setOpen] = useState(hasActiveChild);

  useEffect(() => {
    if (hasActiveChild) {
      setOpen(true);
    }
  }, [hasActiveChild]);

  const groupId = `nav-group-${item.id}`;

  return (
    <div className="sidebar-group">
      {item.path ? (
        <div className="sidebar-group-row">
          <NavLink
            to={item.path}
            className={({ isActive }) =>
              `sidebar-link sidebar-group-link${isActive ? ' is-active' : ''}`
            }
            end
          >
            {item.label}
          </NavLink>
          <button
            type="button"
            className="sidebar-toggle"
            aria-expanded={open}
            aria-controls={groupId}
            aria-label={`${item.label} alt menüyü ${open ? 'kapat' : 'aç'}`}
            onClick={() => setOpen((value) => !value)}
          >
            <ChevronIcon open={open} />
          </button>
        </div>
      ) : (
        <button
          type="button"
          className={`sidebar-group-button${hasActiveChild ? ' is-active' : ''}`}
          aria-expanded={open}
          aria-controls={groupId}
          onClick={() => setOpen((value) => !value)}
        >
          <span>{item.label}</span>
          <ChevronIcon open={open} />
        </button>
      )}

      {open ? (
        <div id={groupId} className="sidebar-children" role="group">
          {children.map((child) => (
            <NavNode key={child.id} item={child} />
          ))}
        </div>
      ) : null}
    </div>
  );
}

function NavNode({ item }: { item: NavItem }) {
  const hasChildren = Boolean(item.children?.length);

  if (hasChildren) {
    return <NavGroup item={item} />;
  }

  return <NavLeaf item={item} />;
}

export function Sidebar() {
  const { role } = useAuth();
  const visibleItems = useMemo(() => filterNavItems(navItems, role), [role]);

  return (
    <aside className="sidebar" aria-label="Ana menü">
      <nav className="sidebar-nav">
        {visibleItems.map((item) => (
          <NavNode key={item.id} item={item} />
        ))}
      </nav>
    </aside>
  );
}
