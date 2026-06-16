import { get, post, put, del } from './request';
import type { ApprovalInstance, ApprovalTemplate } from '@/types/approval.d';
import type { PageParams, PagedResult } from '@/types/api.d';

export const approvalApi = {
  // ========== 流程模板管理（后端路由: api/approval/flow）==========
  /** 获取流程列表 */
  templateList: (params: PageParams & { moduleType?: string }) => get<PagedResult<ApprovalTemplate>>('/approval/flow/list', params),
  /** 创建流程 */
  templateAdd: (data: Partial<ApprovalTemplate>) => post('/approval/flow', data),
  /** 修改流程 */
  templateUpdate: (data: Partial<ApprovalTemplate>) => put(`/approval/flow/${data.id}`, data),
  /** 删除流程 */
  templateRemove: (id: number) => del(`/approval/flow/${id}`),

  // ========== 审批实例（后端路由: api/approval/instance）==========
  /** 分页查询审批实例 (status: 0=待审批 1=通过 2=驳回 3=撤回) */
  list: (params: PageParams & { status?: number; moduleType?: string }) => get<PagedResult<ApprovalInstance>>('/approval/instance/list', params),
  /** 待办（status=0） */
  pending: (params: PageParams) => get<PagedResult<ApprovalInstance>>('/approval/instance/list', { ...params, status: 0 }),
  /** 已办（status=1） */
  done: (params: PageParams) => get<PagedResult<ApprovalInstance>>('/approval/instance/list', { ...params, status: 1 }),
  /** 我的申请（我发起的） */
  my: (params: PageParams) => get<PagedResult<ApprovalInstance>>('/approval/instance/list', { ...params, status: undefined }),
  /** 我的待办（我需要审批的） */
  myPending: (moduleType?: string) => get<ApprovalInstance[]>('/approval/instance/my-pending', { moduleType }),
  /** 我的已办（我已经审批过的） */
  myDone: (moduleType?: string) => get<ApprovalInstance[]>('/approval/instance/my-done', { moduleType }),
  /** 我的申请（我发起的） */
  myInitiated: (params: PageParams & { moduleType?: string }) => get<PagedResult<ApprovalInstance>>('/approval/instance/my-initiated', params),
  /** 获取审批详情 */
  detail: (id: number) => get<ApprovalInstance>(`/approval/instance/${id}`),
  /** 发起审批 */
  start: (data: Partial<ApprovalInstance>) => post('/approval/instance/start', data),
  /** 审批操作（通过/驳回/转办） */
  action: (id: number, actionType: string, comment: string, targetUserId?: number) =>
    post('/approval/instance/action', { instanceId: id, actionType, comment, targetUserId }),
  /** 通过审批（便捷方法） */
  approve: (id: number, comment: string) => post('/approval/instance/action', { instanceId: id, actionType: 'approve', comment }),
  /** 驳回审批（便捷方法） */
  reject: (id: number, comment: string) => post('/approval/instance/action', { instanceId: id, actionType: 'reject', comment }),
  /** 撤回审批 */
  withdraw: (id: number) => post(`/approval/instance/${id}/withdraw`),
  /** 获取审批记录 */
  records: (instanceId: number) => get<{ nodeIndex: number; nodeName: string; approverName: string; action: number; comment: string; createTime: string }[]>(`/approval/instance/${instanceId}/records`),
  /** 转办审批 */
  transfer: (instanceId: number, targetUserId: number, comment: string) =>
    post(`/approval/instance/${instanceId}/transfer`, { targetUserId, comment }),
  /** 批量审批操作 */
  batchAction: (ids: number[], actionType: string, comment: string) =>
    post('/approval/instance/batch-action', { ids, actionType, comment }),
  /** 获取审批统计数据 */
  statistics: () => get<Record<string, unknown>>('/approval/instance/statistics'),
};
