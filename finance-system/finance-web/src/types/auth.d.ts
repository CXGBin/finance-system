/** 登录请求 */
export interface LoginParams {
  username: string;
  password: string;
  remember?: boolean;
}

/** 登录响应 */
export interface LoginResult {
  token: string;
  refreshToken: string;
  userInfo: UserInfo;
}

/** 用户信息 */
export interface UserInfo {
  id: number;
  username: string;
  nickname: string;
  avatar?: string;
  email?: string;
  phone?: string;
  roles: Role[];
  permissions: string[];
  modules: ModuleItem[];
}

/** 角色 */
export interface Role {
  id: number;
  name: string;
  code: string;
}

/** 模块项 */
export interface ModuleItem {
  id: number;
  name: string;
  code: string;
  icon?: string;
  enabled: boolean;
  sort?: number;
}
