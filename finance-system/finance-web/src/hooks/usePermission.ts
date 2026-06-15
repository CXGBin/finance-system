import { usePermissionStore } from '@/store/usePermissionStore';

export function usePermission() {
  const hasPermission = usePermissionStore((s) => s.hasPermission);
  const hasAnyPermission = usePermissionStore((s) => s.hasAnyPermission);

  return { hasPermission, hasAnyPermission };
}
