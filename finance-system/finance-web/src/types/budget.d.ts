/** 预算项 */
export interface BudgetItem {
  id: number;
  year: number;
  subjectId: number;
  subjectCode?: string;
  subjectName?: string;
  deptId?: number;
  deptName?: string;
  month: number;
  amount: number;
  description?: string;
}

/** 预算执行记录 */
export interface BudgetExecution {
  budgetId: number;
  budgetAmount: number;
  actualAmount: number;
  executionRate: number;
  remainAmount: number;
  alertLevel?: 'normal' | 'warning' | 'danger';
}

/** 预算月度数据 */
export interface BudgetMonthly {
  id: number;
  budgetSubjectId: number;
  month: number;
  amount: number;
}

/** 预算月度保存请求 */
export interface BudgetMonthlySaveRequest {
  budgetSubjectId: number;
  items: BudgetMonthly[];
}

/** 预算调整 */
export interface BudgetAdjust {
  id: number;
  budgetId: number;
  adjustAmount: number;
  reason: string;
  status: number;
  createTime?: string;
}

/** 预算预警 */
export interface BudgetAlert {
  id: number;
  subjectName: string;
  deptName: string;
  budgetAmount: number;
  actualAmount: number;
  executionRate: number;
  threshold: number;
  level: 'warning' | 'danger';
  createTime: string;
}
