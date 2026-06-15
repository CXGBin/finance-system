import dayjs from 'dayjs';

/** 金额千分位格式化 */
export function formatMoney(value: number | string | undefined | null, precision = 2): string {
  if (value === null || value === undefined || value === '') return '-';
  const num = typeof value === 'string' ? parseFloat(value) : value;
  if (isNaN(num)) return '-';
  const fixed = Math.abs(num).toFixed(precision);
  const parts = fixed.split('.');
  parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ',');
  return num < 0 ? `-${parts.join('.')}` : parts.join('.');
}

/** 日期格式化 */
export function formatDate(date: string | undefined | null, fmt = 'YYYY-MM-DD'): string {
  if (!date) return '-';
  return dayjs(date).format(fmt);
}

/** 日期时间格式化 */
export function formatDateTime(date: string | undefined | null): string {
  return formatDate(date, 'YYYY-MM-DD HH:mm:ss');
}

/** 会计期间显示 */
export function formatPeriod(period: string): string {
  if (!period) return '-';
  const [year, month] = period.split('-');
  return `${year}年${parseInt(month, 10)}月`;
}

/** 获取负数样式 */
export function getAmountColor(value: number | string | undefined | null): string {
  if (value === null || value === undefined || value === '') return '';
  const num = typeof value === 'string' ? parseFloat(value) : value;
  return num < 0 ? 'color: #ff4d4f' : '';
}

/** 会计科目类型标签颜色 */
export function getSubjectTypeColor(type: number): string {
  const colorMap: Record<number, string> = {
    1: 'blue',
    2: 'orange',
    3: 'green',
    4: 'purple',
    5: 'cyan',
  };
  return colorMap[type] || 'default';
}
