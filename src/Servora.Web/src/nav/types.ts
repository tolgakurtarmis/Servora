import type { Role } from '@/api/types';

export type NavItem = {
  id: string;
  label: string;
  /** Leaf link path. Omit when the item is only a group header. */
  path?: string;
  /** Nested items. Omit or leave empty for a direct selectable option. */
  children?: NavItem[];
  /** If set, item is visible only for these roles. */
  roles?: readonly Role[];
};
