/** 会计科目 */
export interface Subject {
  id: number;
  code: string;
  name: string;
  parentId?: number;
  level: number;
  type: number; // 1资产 2负债 3权益 4成本 5损益
  balanceDirection: 'debit' | 'credit';
  status: number;
  children?: Subject[];
}

/** 期初余额 */
export interface BalanceItem {
  id: number;
  subjectId: number;
  subjectCode: string;
  subjectName: string;
  yearPeriod: string;
  debitAmount: number;
  creditAmount: number;
  debitQuantity?: number;
  creditQuantity?: number;
}

/** 凭证头 */
export interface Voucher {
  id: number;
  voucherNo: string;
  voucherDate: string;
  period: string;
  type: number; // 凭证字
  summary?: string;
  totalDebit: number;
  totalCredit: number;
  status: number; // 0草稿 1已审核 2已作废
  entryCount: number;
  creator?: string;
  auditor?: string;
  createTime?: string;
  entries: VoucherEntry[];
}

/** 凭证分录 */
export interface VoucherEntry {
  id?: number;
  subjectId: number;
  subjectCode?: string;
  subjectName?: string;
  summary: string;
  debitAmount: number;
  creditAmount: number;
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

/** 会计期间 */
export interface AccountingPeriod {
  year: number;
  month: number;
  period: string; // YYYY-MM
  status: 'open' | 'closed' | 'locked';
  beginDate: string;
  endDate: string;
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
