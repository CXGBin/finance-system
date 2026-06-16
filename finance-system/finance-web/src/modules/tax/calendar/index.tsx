import React from 'react';
import { Card } from 'antd';
import { Calendar as AntCalendar } from 'antd';

/** 税务日历 */
const TaxCalendar: React.FC = () => {
  return (
    <Card title="税务日历">
      <AntCalendar />
    </Card>
  );
};

export default TaxCalendar;
