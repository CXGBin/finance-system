/** 登录请求 */
export interface LoginParams {
  username: string;
  password: string;
  rememberMe?: boolean;
}

/** 登录响应 - 与后端 LoginResponse 对齐 */
export interface LoginResult {
  accessToken: string;
  refreshToken: string;
  expiresIn: number;
  userInfo: UserInfo;
  mustChangePassword: boolean;
}

/** 用户信息 - 与后端 UserInfoDto 对齐 */
export interface UserInfo {
  id: number;
  username: string;
  realName: string;
  avatar?: string;
  roles: string[]; // 后端返回角色编码列表
  permissions: string[];
  modules: string[]; // 后端返回模块标识列表
}

/** 角色（用于角色管理页面） */
export interface Role {
  id: number;
  roleName: string;
  roleCode: string;
  description?: string;
  sortOrder?: number;
  status: number;
  dataScope?: number;
  menuIds?: number[];
  createTime?: string;
}
