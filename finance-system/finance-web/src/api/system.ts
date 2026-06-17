import { get, post, put, del } from './request';
import type { User, SysRole, Menu, Dept, Post, DictType, DictItem, OperLog, SysModule, SysConfig, SysNotice } from '@/types/system.d';
import type { PageParams, PagedResult } from '@/types/api.d';

// 用户管理
export const userApi = {
  /** 分页查询用户列表 */
  page: (params: PageParams & Partial<User>) => get<PagedResult<User>>('/system/user/page', params),
  /** 查询用户详情 */
  detail: (id: number) => get<User>(`/system/user/${id}`),
  /** 新增用户 */
  add: (data: Partial<User>) => post('/system/user', data),
  /** 编辑用户 */
  update: (data: Partial<User>) => put(`/system/user/${data.id}`, data),
  /** 删除用户 */
  remove: (id: number) => del(`/system/user/${id}`),
  /** 切换用户状态 */
  toggleStatus: (id: number, status: number) => put(`/system/user/${id}/status`, null, { params: { status } }),
  /** 重置用户密码 */
  resetPassword: (id: number) => put(`/system/user/${id}/reset-password`),
  /** 获取个人信息 */
  profile: () => get<User>('/system/user/profile'),
  /** 修改个人信息 */
  updateProfile: (data: Partial<User>) => put('/system/user/profile', data),
  /** 兼容旧调用（别名） */
  list: (params: PageParams & Partial<User>) => get<PagedResult<User>>('/system/user/page', params),
};

// 角色管理
export const roleApi = {
  /** 分页查询角色列表 */
  page: (params: PageParams & Partial<SysRole>) => get<PagedResult<SysRole>>('/system/role/page', params),
  /** 获取所有启用角色列表（下拉） */
  all: () => get<SysRole[]>('/system/role/list'),
  /** 查询角色详情 */
  detail: (id: number) => get<SysRole>(`/system/role/${id}`),
  /** 新增角色 */
  add: (data: Partial<SysRole>) => post('/system/role', data),
  /** 编辑角色 */
  update: (data: Partial<SysRole>) => put(`/system/role/${data.id}`, data),
  /** 删除角色 */
  remove: (id: number) => del(`/system/role/${id}`),
  /** 获取角色已分配菜单 */
  menus: (id: number) => get<number[]>(`/system/role/${id}/menus`),
  /** 保存角色菜单分配 */
  saveMenus: (id: number, menuIds: number[]) => put(`/system/role/${id}/menus`, menuIds),
  /** 兼容旧调用（别名） */
  list: (params: PageParams & Partial<SysRole>) => get<PagedResult<SysRole>>('/system/role/page', params),
};

// 菜单管理
export const menuApi = {
  tree: () => get<Menu[]>('/system/menu/tree'),
  detail: (id: number) => get<Menu>(`/system/menu/${id}`),
  add: (data: Partial<Menu>) => post('/system/menu', data),
  update: (data: Partial<Menu>) => put(`/system/menu/${data.id}`, data),
  remove: (id: number) => del(`/system/menu/${id}`),
};

// 部门管理
export const deptApi = {
  tree: () => get<Dept[]>('/system/dept/tree'),
  add: (data: Partial<Dept>) => post('/system/dept', data),
  update: (data: Partial<Dept>) => put(`/system/dept/${data.id}`, data),
  remove: (id: number) => del(`/system/dept/${id}`),
};

// 岗位管理
export const postApi = {
  list: (params: PageParams & Partial<Post>) => get<PagedResult<Post>>('/system/post/page', params),
  add: (data: Partial<Post>) => post('/system/post', data),
  update: (data: Partial<Post>) => put(`/system/post/${data.id}`, data),
  remove: (id: number) => del(`/system/post/${id}`),
};

// 数据字典
export const dictApi = {
  /** 分页查询字典类型 */
  typePage: (params: PageParams) => get<PagedResult<DictType>>('/system/dict/type/page', params),
  /** 新增字典类型 */
  typeAdd: (data: Partial<DictType>) => post('/system/dict/type', data),
  /** 编辑字典类型 */
  typeUpdate: (data: Partial<DictType>) => put(`/system/dict/type/${data.id}`, data),
  /** 删除字典类型 */
  typeRemove: (id: number) => del(`/system/dict/type/${id}`),
  /** 获取某类型下所有字典项 */
  itemList: (dictType: string) => get<DictItem[]>(`/system/dict/data/${dictType}`),
  /** 新增字典项 */
  itemAdd: (data: Partial<DictItem>) => post('/system/dict/data', data),
  /** 编辑字典项 */
  itemUpdate: (data: Partial<DictItem>) => put(`/system/dict/data/${data.id}`, data),
  /** 删除字典项 */
  itemRemove: (id: number) => del(`/system/dict/data/${id}`),
  /** 兼容旧调用（别名） */
  typeList: () => get<DictType[]>('/system/dict/type/page', { pageIndex: 1, pageSize: 999 }),
};

// 操作日志
export const logApi = {
  /** 分页查询操作日志 */
  page: (params: PageParams & Partial<OperLog>) => get<PagedResult<OperLog>>('/system/log/page', params),
  /** 查看日志详情 */
  detail: (id: number) => get<OperLog>(`/system/log/${id}`),
  /** 兼容旧调用（别名） */
  list: (params: PageParams & Partial<OperLog>) => get<PagedResult<OperLog>>('/system/log/page', params),
};

// 模块管理
export const moduleApi = {
  /** 获取模块列表 */
  list: () => get<SysModule[]>('/system/module/list'),
  /** 切换模块开关 */
  toggle: (moduleId: string, enabled: boolean) => put(`/system/module/${moduleId}/toggle`, { isEnabled: enabled }),
  /** 获取模块依赖关系 */
  dependencies: (moduleId: string) => get<string[]>(`/system/module/${moduleId}/dependencies`),
  /** 兼容旧调用（别名） */
  update: (id: number | string, enabled: boolean) => put(`/system/module/${id}/toggle`, { isEnabled: enabled }),
};

// 系统配置
export const configApi = {
  /** 获取所有配置项 */
  list: (group?: string) => get<SysConfig[]>('/system/config/list', { group }),
  /** 批量修改配置 */
  batchUpdate: (items: Partial<SysConfig>[]) => put('/system/config', items),
  /** 兼容旧调用（别名） */
  update: (data: { configKey: string; configValue: string }) => put(`/system/config/${data.configKey}`, data),
};

/** 系统公告API */
export const noticeApi = {
  /** 获取公告列表 */
  list: (noticeType?: number, params?: PageParams & Record<string, unknown>) => get<PagedResult<SysNotice>>(`/system/notice/list`, { noticeType, ...params }),
  /** 新增公告 */
  create: (data: Omit<SysNotice, 'id' | 'createdTime'>) => post<number>('/system/notice', data),
  /** 修改公告 */
  update: (id: number, data: Partial<SysNotice>) => put(`/system/notice/${id}`, data),
  /** 删除公告 */
  remove: (id: number) => del(`/system/notice/${id}`),
};
