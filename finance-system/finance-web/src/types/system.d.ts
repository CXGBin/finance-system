/** 用户 - 与后端 SysUser/UserProfile 对齐 */
export interface User {
  id: number;
  username: string;
  realName: string; // 后端 RealName
  email?: string;
  phone?: string;
  avatar?: string;
  status: number; // 0禁用 1启用
  deptId?: number;
  deptName?: string;
  postId?: number; // 后端返回单个岗位ID
  roleIds: number[];
  roleNames?: string[];
  createdTime?: string; // 后端 CreatedTime
  remark?: string;
}

/** 角色 - 与后端 SysRole 对齐 */
export interface SysRole {
  id: number;
  roleName: string; // 后端 RoleName
  roleCode: string; // 后端 RoleCode
  description?: string;
  sortOrder?: number;
  status: number;
  dataScope?: number;
  menuIds?: number[];
  createdTime?: string; // 后端 CreatedTime
}

/** 菜单 - 与后端 SysMenu 对齐 */
export interface Menu {
  id: number;
  parentId: number;
  menuName: string; // 后端 MenuName
  path?: string;
  component?: string;
  icon?: string;
  permission?: string;
  moduleId?: string;
  sortOrder: number; // 后端 SortOrder
  menuType: number; // 后端 MenuType (1目录 2菜单 3按钮)
  visible: number; // 后端 Visible (0隐藏 1显示)
  status: number;
  children?: Menu[];
}

/** 部门 - 与后端 SysDept 对齐 */
export interface Dept {
  id: number;
  parentId: number;
  deptName: string; // 后端 DeptName
  sortOrder: number; // 后端 SortOrder
  leader?: string;
  phone?: string;
  email?: string;
  status: number;
  children?: Dept[];
}

/** 岗位 - 与后端 SysPost 对齐 */
export interface Post {
  id: number;
  deptId: number;
  postCode: string; // 后端 PostCode
  postName: string; // 后端 PostName
  sortOrder: number; // 后端 SortOrder
  status: number;
  remark?: string;
  createdTime?: string;
}

/** 数据字典类型 - 与后端 SysDictType 对齐 */
export interface DictType {
  id: number;
  dictName: string; // 后端 DictName
  dictType: string; // 后端 DictType
  status: number;
  remark?: string;
  createdTime?: string;
}

/** 数据字典项 - 与后端 SysDictData 对齐 */
export interface DictItem {
  id: number;
  dictType: string; // 后端 DictType (string类型)
  dictLabel: string; // 后端 DictLabel
  dictValue: string; // 后端 DictValue
  sortOrder: number; // 后端 SortOrder
  status: number;
  remark?: string;
}

/** 操作日志 - 与后端 SysLog 对齐 */
export interface OperLog {
  id: number;
  userId: number;
  userName: string; // 后端 UserName
  module?: string;
  action?: string;
  description?: string;
  ipAddress?: string;
  requestUrl?: string;
  requestMethod?: string;
  requestBody?: string;
  responseCode?: number;
  durationMs: number;
  createdTime: string; // 后端 CreatedTime
}

/** 模块配置 - 与后端 SysModule 对齐 */
export interface SysModule {
  id: number;
  moduleId: string; // 后端 ModuleId
  moduleName: string; // 后端 ModuleName
  description?: string;
  isEnabled: boolean; // 后端 IsEnabled
  isCore: boolean; // 后端 IsCore
  sortOrder?: number;
}

/** 系统配置项 - 与后端 SysConfig 对齐 */
export interface SysConfig {
  configKey: string; // 后端 ConfigKey
  configValue: string; // 后端 ConfigValue
  configName: string; // 后端 ConfigName
  configGroup: string; // 后端 ConfigGroup
  remark?: string;
}
