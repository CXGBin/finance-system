import React, { useState } from 'react';
import { Card, Select, Button, Calendar, Badge } from 'antd';
import { taxApi } from '@/api/tax';
import dayjs from 'dayjs';

/** 税务日历 */
const TaxCalendar: React.FC = () => {
  const [month, setMonth] = useState(dayjs().format('YYYY-MM'));
  const [events, setEvents] = useState<any[]>([]);

  React.useEffect(() => {
    taxApi.calendar(month).then(res => setEvents(res.data || []));
  }, [month]);

  const dateCellRender = (date: dayjs.Dayjs) => {
    const dayStr = date.format('YYYY-MM-DD');
    const dayEvents = events.filter((e: TaxCalendarEvent) => e.date === dayStr);
    return (
      <ul style={{ listStyle: 'none', padding: 0, margin: 0 }}>
        {dayEvents.map((e: TaxCalendarEvent, i: number) => (
          <li key={i}><Badge status={e.isDone ? 'success' : 'processing'} text={e.taxName} /></li>
        ))}
      </ul>
    );
  };

  return (
    <Card title="税务日历">
      <Select value={month} onChange={setMonth} style={{ width: 120, marginBottom: 16 }} options={Array.from({ length: 12 }, (_, i) => ({ label: `${dayjs().year()}-${String(i + 1).padStart(2, '0')}`, value: `${dayjs().year()}-${String(i + 1).padStart(2, '0')}` }))} />
      <Calendar cellRender={(date, info) => { if (info.type === 'date') return dateCellRender(date as dayjs.Dayjs); return null; }} />
    </Card>
  );
};

export default TaxCalendar;
