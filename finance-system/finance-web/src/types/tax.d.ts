/** 税种 */
export interface TaxType {
  id: number;
  name: string;
  code: string;
  rate: number;
  type: 'vat' | 'income' | 'property' | 'stamp' | 'other';
  status: number;
  description?: string;
}

/** 纳税申报 */
export interface TaxDeclaration {
  id: number;
  taxTypeId: number;
  taxTypeName?: string;
  period: string;
  taxAmount: number;
  status: 'draft' | 'submitted' | 'paid';
  declareDate?: string;
  createTime: string;
}

/** 发票 */
export interface Invoice {
  id: number;
  invoiceNo: string;
  invoiceType: string;
  invoiceDate: string;
  amount: number;
  tax: number;
  totalAmount: number;
  direction: 'in' | 'out';
  status: 'normal' | 'red' | 'void';
  vendor?: string;
  remark?: string;
}

/** 税务报表 */
export interface TaxReport {
  taxType: string;
  period: string;
  taxBase: number;
  taxAmount: number;
  paidAmount: number;
  unpaidAmount: number;
}

/** 税务日历项 */
export interface TaxCalendarItem {
  id: number;
  taxType: string;
  period: string;
  deadline: string;
  status: 'pending' | 'submitted' | 'overdue';
}
