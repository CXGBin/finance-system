import { create } from 'zustand';

interface PermissionState {
  permissions: string[];
  setPermissions: (permissions: string[]) => void;
  hasPermission: (perm: string) => boolean;
  hasAnyPermission: (perms: string[]) => boolean;
}

export const usePermissionStore = create<PermissionState>((set, get) => ({
  permissions: [],
  setPermissions: (permissions) => set({ permissions }),
  hasPermission: (perm) => {
    const { permissions } = get();
    return permissions.includes('*') || permissions.includes(perm);
  },
  hasAnyPermission: (perms) => perms.some((p) => get().hasPermission(p)),
}));
