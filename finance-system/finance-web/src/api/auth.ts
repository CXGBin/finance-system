import { post, put } from './request';
import type { LoginParams, LoginResult } from '@/types/auth.d';

/** 登录 */
export function login(data: LoginParams) {
  return post<LoginResult>('/auth/login', data);
}

/** 登出 */
export function logout() {
  return post('/auth/logout');
}

/** 刷新Token */
export function refreshToken(refreshToken: string) {
  return post<{ token: string; refreshToken: string }>('/auth/refresh-token', { refreshToken });
}

/** 获取当前用户信息 */
export function getUserInfo() {
  return post<LoginResult['userInfo']>('/auth/userinfo');
}

/** 修改密码 */
export function changePassword(data: { oldPassword: string; newPassword: string }) {
  return put('/auth/password', data);
}
