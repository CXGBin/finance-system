/** 审批节点 */
export interface ApprovalNode {
  id: number;
  name: string;
  type: 'start' | 'approve' | 'cc' | 'end';
  status: 'pending' | 'approved' | 'rejected' | 'transfer';
  assignee?: string;
  comment?: string;
  completeTime?: string;
}

/** 审批实例 */
export interface ApprovalInstance {
  id: number;
  title: string;
  type: string;
  typeName?: string;
  applicant: string;
  applicantId: number;
  status: 'pending' | 'approved' | 'rejected' | 'withdrawn' | 'cancelled';
  currentNode?: string;
  createTime: string;
  completeTime?: string;
  nodes: ApprovalNode[];
  formUrl?: string;
}

/** 审批模板 */
export interface ApprovalTemplate {
  id: number;
  name: string;
  code: string;
  formType: string;
  nodeConfig: ApprovalNode[];
  status: number;
  remark?: string;
}
