/** 用户 */
export interface User {
  id: number;
  username: string;
  nickname: string;
  email?: string;
  phone?: string;
  avatar?: string;
  status: number; // 0禁用 1启用
  deptId?: number;
  deptName?: string;
  postIds?: number[];
  roleIds: number[];
  roleNames?: string[];
  createTime?: string;
  remark?: string;
}

/** 角色 */
export interface SysRole {
  id: number;
  name: string;
  code: string;
  sort?: number;
  status: number;
  remark?: string;
  menuIds?: number[];
  createTime?: string;
}

/** 菜单 */
export interface Menu {
  id: number;
  parentId: number;
  name: string;
  path?: string;
  component?: string;
  icon?: string;
  sort: number;
  type: number; // 0目录 1菜单 2按钮
  status: number;
  children?: Menu[];
  permission?: string;
  visible?: boolean;
}

/** 部门 */
export interface Dept {
  id: number;
  parentId: number;
  name: string;
  sort: number;
  leader?: string;
  phone?: string;
  email?: string;
  status: number;
  children?: Dept[];
}

/** 岗位 */
export interface Post {
  id: number;
  code: string;
  name: string;
  sort: number;
  status: number;
  createTime?: string;
}

/** 数据字典类型 */
export interface DictType {
  id: number;
  name: string;
  code: string;
  status: number;
  remark?: string;
  createTime?: string;
}

/** 数据字典项 */
export interface DictItem {
  id: number;
  dictId: number;
  label: string;
  value: string;
  sort: number;
  status: number;
  remark?: string;
}

/** 操作日志 */
export interface OperLog {
  id: number;
  userId: number;
  userName?: string;
  module?: string;
  action?: string;
  description?: string;
  ipAddress?: string;
  requestUrl?: string;
  requestMethod?: string;
  requestBody?: string;
  responseCode?: number;
  durationMs: number;
  createdTime: string;
}

/** 模块配置 */
export interface SysModule {
  id: number;
  name: string;
  code: string;
  icon?: string;
  enabled: boolean;
  sort?: number;
  description?: string;
}

/** 系统配置项 */
export interface SysConfig {
  key: string;
  value: string;
  label: string;
  group: string;
  type: 'input' | 'number' | 'switch' | 'select';
  options?: { label: string; value: string }[];
}
