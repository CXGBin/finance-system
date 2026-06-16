/** 审批节点 - 与后端 ApprovalRecord 对齐 */
export interface ApprovalNode {
  instanceId?: number; // 后端 InstanceId
  nodeIndex: number; // 后端 NodeIndex
  nodeName: string; // 后端 NodeName
  approverId?: number; // 后端 ApproverId
  action?: string; // 后端 Action (approved/rejected)
  comment?: string;
  approveTime?: string; // 后端 ApproveTime
}

/** 审批实例 - 与后端 ApprovalInstance 对齐 */
export interface ApprovalInstance {
  id: number;
  flowId?: number; // 后端 FlowId
  businessId?: number; // 后端 BusinessId
  moduleType?: string; // 后端 ModuleType
  title: string;
  initiatorId: number; // 后端 InitiatorId
  currentNodeIndex: number; // 后端 CurrentNodeIndex
  status: number; // 0待审批 1已通过 2已驳回 3已撤回
  deptId?: number;
  createTime?: string;
  nodes: ApprovalNode[];
}

/** 审批模板 - 与后端 ApprovalFlow 对齐 */
export interface ApprovalTemplate {
  id: number;
  flowName: string; // 后端 FlowName
  flowCode: string; // 后端 FlowCode
  moduleType?: string; // 后端 ModuleType
  description?: string;
  isEnabled: number; // 后端 IsEnabled
  nodesJson?: string; // 后端 NodesJson
}
