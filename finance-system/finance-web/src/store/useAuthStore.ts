import { create } from 'zustand';
import type { UserInfo } from '@/types/auth.d';
import { getToken, setToken, removeToken, getRefreshToken, setRefreshToken, removeRefreshToken } from '@/utils/token';

interface AuthState {
  token: string | null;
  refreshToken: string | null;
  userInfo: UserInfo | null;
  setAuth: (token: string, refreshToken: string, userInfo: UserInfo) => void;
  logout: () => void;
}

export const useAuthStore = create<AuthState>((set) => ({
  token: getToken(),
  refreshToken: getRefreshToken(),
  userInfo: null,
  setAuth: (token, refreshToken, userInfo) => {
    setToken(token);
    setRefreshToken(refreshToken);
    set({ token, refreshToken, userInfo });
  },
  logout: () => {
    removeToken();
    removeRefreshToken();
    set({ token: null, refreshToken: null, userInfo: null });
  },
}));
