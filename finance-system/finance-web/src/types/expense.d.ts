/** 费用类型 */
export interface ExpenseType {
  id: number;
  name: string;
  code: string;
  budgetSubject?: string;
  sort: number;
  status: number;
}

/** 报销单 */
export interface ExpenseClaim {
  id: number;
  claimNo: string;
  title: string;
  typeId?: number;
  typeName?: string;
  totalAmount: number;
  status: 'draft' | 'submitted' | 'approving' | 'approved' | 'rejected' | 'paid';
  applicant: string;
  applicantId: number;
  createTime: string;
  items: ExpenseClaimItem[];
  attachments?: string[];
}

/** 报销明细 */
export interface ExpenseClaimItem {
  id?: number;
  expenseDate: string;
  description: string;
  amount: number;
  invoiceNo?: string;
  invoiceType?: string;
}

/** 付款记录 */
export interface PaymentRecord {
  id: number;
  claimId: number;
  claimNo: string;
  payAmount: number;
  payMethod: string;
  payDate: string;
  status: 'pending' | 'paid' | 'failed';
}

/** 费用分摊 */
export interface ExpenseAllocate {
  id: number;
  sourceId: number;
  sourceName: string;
  targetDeptId: number;
  targetDeptName: string;
  amount: number;
  ratio: number;
  period: string;
}

/** 费用统计 */
export interface ExpenseStats {
  deptName: string;
  typeName: string;
  totalAmount: number;
  budgetAmount: number;
  executionRate: number;
}
