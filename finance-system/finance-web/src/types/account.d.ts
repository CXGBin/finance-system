/** 会计科目 - 与后端 AccountSubject 对齐 */
export interface Subject {
  id: number;
  subjectCode: string; // 后端 SubjectCode
  subjectName: string; // 后端 SubjectName
  parentId?: number;
  subjectLevel: number; // 后端 SubjectLevel
  subjectType: number; // 后端 SubjectType (1资产 2负债 3权益 4成本 5损益)
  balanceDirection: number; // 后端 BalanceDirection (1借方 2贷方)
  isEnabled: number; // 后端 IsEnabled
  isCash?: number;
  isBank?: number;
  auxiliaryType?: string;
  sortOrder?: number;
  remark?: string;
  children?: Subject[];
}

/** 期初余额 - 与后端 SubjectBalance 对齐 */
export interface BalanceItem {
  id: number;
  subjectId: number;
  periodId: number; // 后端 PeriodId
  beginDebit: number; // 后端 BeginDebit
  beginCredit: number; // 后端 BeginCredit
  currentDebit?: number; // 后端 CurrentDebit
  currentCredit?: number; // 后端 CurrentCredit
  endDebit?: number; // 后端 EndDebit
  endCredit?: number; // 后端 EndCredit
}

/** 凭证头 - 与后端 Voucher 对齐 */
export interface Voucher {
  id: number;
  voucherNo: string; // 后端 VoucherNo
  voucherDate: string; // 后端 VoucherDate
  periodId?: number; // 后端 PeriodId
  voucherType: number; // 后端 VoucherType
  abstractText?: string; // 后端 AbstractText
  status: number; // 0草稿 1已审核 2已作废
  totalDebit: number; // 后端 TotalDebit
  totalCredit: number; // 后端 TotalCredit
  preparedBy?: number; // 后端 PreparedBy
  reviewedBy?: number; // 后端 ReviewedBy
  reviewedTime?: string; // 后端 ReviewedTime
  entries: VoucherEntry[];
}

/** 凭证分录 - 与后端 VoucherEntry 对齐 */
export interface VoucherEntry {
  id?: number;
  subjectId: number; // 后端 SubjectId
  subjectCode?: string;
  subjectName?: string;
  summary: string; // 后端 Summary
  debitAmount: number; // 后端 DebitAmount
  creditAmount: number; // 后端 CreditAmount
  auxiliaryId?: number;
  auxiliaryType?: string;
}

/** 总账/明细账/日记账记录 */
export interface LedgerRecord {
  period: string;
  voucherNo: string;
  voucherDate: string;
  summary: string;
  debitAmount: number;
  creditAmount: number;
  balance: number;
  direction: 'debit' | 'credit';
}

/** 会计期间 - 与后端 AccountingPeriod 对齐 */
export interface AccountingPeriod {
  id: number;
  periodYear: number; // 后端 PeriodYear
  periodMonth: number; // 后端 PeriodMonth
  beginDate: string;
  endDate: string;
  isClosed: boolean; // 后端 IsClosed
  closedTime?: string;
  closedBy?: number;
}

/** 会计科目类型枚举值 */
export const SubjectTypeMap: Record<number, string> = {
  1: '资产',
  2: '负债',
  3: '所有者权益',
  4: '成本',
  5: '损益',
};

/** 凭证状态枚举值 */
export const VoucherStatusMap: Record<number, { text: string; color: string }> = {
  0: { text: '草稿', color: 'default' },
  1: { text: '已审核', color: 'green' },
  2: { text: '已作废', color: 'red' },
};
