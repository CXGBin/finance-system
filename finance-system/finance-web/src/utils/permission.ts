import { usePermissionStore } from '@/store/usePermissionStore';

/** 校验是否拥有某个权限 */
export function hasPermission(permission: string): boolean {
  const { permissions } = usePermissionStore.getState();
  return permissions.includes('*') || permissions.includes(permission);
}

/** 校验是否拥有某些权限之一 */
export function hasAnyPermission(perms: string[]): boolean {
  return perms.some(hasPermission);
}

/** 校验是否拥有所有指定权限 */
export function hasAllPermissions(perms: string[]): boolean {
  return perms.every(hasPermission);
}
