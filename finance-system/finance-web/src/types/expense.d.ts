/** 费用类型 - 与后端 ExpenseType 对齐 */
export interface ExpenseType {
  id: number;
  typeCode: string; // 后端 TypeCode
  typeName: string; // 后端 TypeName
  subjectId?: number;
  singleLimit?: number;
  monthlyLimit?: number;
  sortOrder: number; // 后端 SortOrder
  isEnabled: number; // 后端 IsEnabled
}

/** 报销单 - 与后端 ExpenseClaim 对齐 */
export interface ExpenseClaim {
  id: number;
  claimNo: string; // 后端 ClaimNo
  title: string;
  claimantId?: number; // 后端 ClaimantId
  deptId?: number; // 后端 DeptId
  totalAmount: number;
  status: number; // 0草稿 1待审批 2已审批 3已驳回 4已付款
  approvalInstanceId?: number;
  paymentDate?: string;
  voucherId?: number;
  remark?: string;
  items: ExpenseClaimItem[];
}

/** 报销明细 - 与后端 ExpenseClaimItem 对齐 */
export interface ExpenseClaimItem {
  id?: number;
  expenseTypeId?: number; // 后端 ExpenseTypeId
  description: string; // 后端 Description
  amount: number; // 后端 Amount
  expenseDate: string; // 后端 ExpenseDate
  invoiceNo?: string;
}

/** 费用分摊 - 与后端 ExpenseAllocate 对齐 */
export interface ExpenseAllocate {
  id: number;
  allocateNo: string; // 后端 AllocateNo
  description?: string;
  amount?: number;
  status?: number;
  createTime?: string;
}

/** 费用统计 */
export interface ExpenseStats {
  deptName: string;
  typeName: string;
  totalAmount: number;
  budgetAmount: number;
  executionRate: number;
}

/** 借款申请 - 与后端 ExpenseLoan 对齐 */
export interface ExpenseLoan {
  id: number;
  loanNo: string; // 后端 LoanNo
  applicantId: number; // 后端 ApplicantId
  loanAmount: number; // 后端 LoanAmount
  settledAmount: number; // 后端 SettledAmount
  reason?: string;
  expectedReturnDate?: string; // 后端 ExpectedReturnDate
  voucherId?: number;
  status: number; // 0待审批 1已借出 2已核销 3已退回
  approvalInstanceId?: number;
  createdTime: string;
  remark?: string;
}
