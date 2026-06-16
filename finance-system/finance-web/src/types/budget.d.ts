/** 预算项 - 与后端 BudgetSubject 对齐 */
export interface BudgetItem {
  id: number;
  budgetYearId: number; // 后端 BudgetYearId
  subjectId: number; // 后端 SubjectId
  deptId?: number; // 后端 DeptId
  annualAmount: number; // 后端 AnnualAmount
  remark?: string;
  createdTime?: string;
}

/** 预算执行记录 - 与后端 BudgetExecutionItem 对齐 */
export interface BudgetExecution {
  budgetSubjectId: number; // 后端 BudgetSubjectId
  subjectCode?: string; // 后端 SubjectCode
  subjectName?: string; // 后端 SubjectName
  deptName?: string; // 后端 DeptName
  annualBudget: number; // 后端 AnnualBudget
  executedAmount: number; // 后端 ExecutedAmount
  executionRate: number; // 后端 ExecutionRate
  remainingBudget: number; // 后端 RemainingBudget
}

/** 预算月度数据 - 与后端 BudgetMonthly 对齐 */
export interface BudgetMonthly {
  id: number;
  budgetSubjectId: number; // 后端 BudgetSubjectId
  month: number;
  amount: number;
}

/** 预算月度保存请求 */
export interface BudgetMonthlySaveRequest {
  budgetSubjectId: number;
  items: BudgetMonthly[];
}

/** 预算调整 - 与后端 BudgetAdjustment 对齐 */
export interface BudgetAdjust {
  id: number;
  budgetSubjectId?: number;
  adjustType: number; // 后端 AdjustType (int: 1增加 2减少)
  beforeAmount: number; // 后端 BeforeAmount
  afterAmount: number; // 后端 AfterAmount
  reason: string;
  approveStatus: number; // 后端 ApproveStatus
  applyDeptId?: number;
  applyBy?: number;
  createTime?: string;
}

/** 预算预警 - 与后端 BudgetAlertConfig/BudgetAlertCheck 对齐 */
export interface BudgetAlert {
  id: number;
  budgetYearId: number; // 后端 BudgetYearId
  alertThreshold: number; // 后端 AlertThreshold
  isEnabled: number; // 后端 IsEnabled
  alertMethod?: number; // 后端 AlertMethod (int)
}
