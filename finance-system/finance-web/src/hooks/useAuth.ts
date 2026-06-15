import { useCallback } from 'react';
import { useAuthStore } from '@/store/useAuthStore';
import { usePermissionStore } from '@/store/usePermissionStore';
import { useModuleStore } from '@/store/useModuleStore';
import { login as loginApi, getUserInfo, logout as logoutApi } from '@/api/auth';

export function useAuth() {
  const { token, userInfo, setAuth, logout: storeLogout } = useAuthStore();
  const setPermissions = usePermissionStore((s) => s.setPermissions);
  const setModules = useModuleStore((s) => s.setModules);

  const login = useCallback(
    async (username: string, password: string, remember?: boolean) => {
      const res = await loginApi({ username, password, remember });
      const { token: t, refreshToken, userInfo: info } = res.data;
      setAuth(t, refreshToken, info);
      if (info.permissions) setPermissions(info.permissions);
      if (info.modules) setModules(info.modules);
      return res.data;
    },
    [setAuth, setPermissions, setModules],
  );

  const logout = useCallback(async () => {
    try {
      if (token) await logoutApi();
    } catch {
      // ignore
    }
    storeLogout();
    usePermissionStore.getState().setPermissions([]);
    useModuleStore.getState().setModules([]);
  }, [token, storeLogout]);

  const fetchUserInfo = useCallback(async () => {
    if (!token) return;
    try {
      const res = await getUserInfo();
      const info = res.data;
      setAuth(token, useAuthStore.getState().refreshToken!, info);
      if (info.permissions) setPermissions(info.permissions);
      if (info.modules) setModules(info.modules);
    } catch {
      // ignore
    }
  }, [token, setAuth, setPermissions, setModules]);

  return { token, userInfo, login, logout, fetchUserInfo };
}
