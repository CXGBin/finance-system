import React, { useState } from 'react';
import { Card, Select, Calendar, Badge } from 'antd';
import { taxApi } from '@/api/tax';
import dayjs from 'dayjs';
import type { Dayjs } from 'dayjs';

/** 税务日历事项类型 */
interface TaxCalendarEvent {
  date: string;
  taxName: string;
  isDone: boolean;
}

/** 税务日历 */
const TaxCalendar: React.FC = () => {
  const [month, setMonth] = useState(dayjs().format('YYYY-MM'));
  const [events, setEvents] = useState<TaxCalendarEvent[]>([]);

  React.useEffect(() => {
    const [yearStr, monthStr] = month.split('-');
    const year = Number(yearStr);
    const m = Number(monthStr);
    taxApi.calendarList(year, m).then(res => setEvents((res.data || []) as TaxCalendarEvent[]));
  }, [month]);

  const dateCellRender = (date: Dayjs) => {
    const dayStr = date.format('YYYY-MM-DD');
    const dayEvents = events.filter((e) => e.date === dayStr);
    return (
      <ul style={{ listStyle: 'none', padding: 0, margin: 0 }}>
        {dayEvents.map((e, i) => (
          <li key={i}><Badge status={e.isDone ? 'success' : 'processing'} text={e.taxName} /></li>
        ))}
      </ul>
    );
  };

  return (
    <Card title="税务日历">
      <Select value={month} onChange={setMonth} style={{ width: 120, marginBottom: 16 }} options={Array.from({ length: 12 }, (_, i) => ({ label: `${dayjs().year()}-${String(i + 1).padStart(2, '0')}`, value: `${dayjs().year()}-${String(i + 1).padStart(2, '0')}` }))} />
      <Calendar cellRender={(date, info) => { if (info.type === 'date') return dateCellRender(date); return null; }} />
    </Card>
  );
};

export default TaxCalendar;
