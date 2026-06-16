import React, { useState } from 'react';
import { Card, Table, Select, Space, Button, message } from 'antd';
import { reportApi, reportExportApi } from '@/api/report';
import type { CashFlowRow } from '@/types/report.d';
import dayjs from 'dayjs';

/** 现金流量表 */
const CashFlow: React.FC = () => {
  const [data, setData] = useState<CashFlowRow[]>([]);
  const [loading, setLoading] = useState(false);
  const [year, setYear] = useState(dayjs().year());
  const [month, setMonth] = useState(dayjs().month() + 1);

  const period = `${year}-${String(month).padStart(2, '0')}`;

  const loadData = async () => {
    setLoading(true);
    try { const res = await reportApi.cashFlow({ period }); setData(res.data || []); } finally { setLoading(false); }
  };

  const handleExport = async () => {
    try {
      message.loading({ content: '正在导出...', key: 'export' });
      const res = await reportExportApi.excel({ reportType: 'cash-flow', period });
      const link = document.createElement('a');
      link.href = res.data as string;
      link.download = `现金流量表_${period}.xlsx`;
      link.click();
      message.success({ content: '导出成功', key: 'export' });
    } catch {
      message.error({ content: '导出失败', key: 'export' });
    }
  };

  const columns = [
    { title: '项目', dataIndex: 'itemName', key: 'itemName', width: 250 },
    { title: '行次', dataIndex: 'lineNo', key: 'lineNo', width: 60, align: 'center' },
    { title: '本期金额', dataIndex: 'currentAmount', key: 'currentAmount', align: 'right' },
    { title: '累计金额', dataIndex: 'totalAmount', key: 'totalAmount', align: 'right' },
  ];

  return (
    <Card title="现金流量表">
      <Space style={{ marginBottom: 16 }}>
        <Select value={year} onChange={setYear} style={{ width: 100 }} options={Array.from({ length: 5 }, (_, i) => ({ label: String(dayjs().year() - 2 + i), value: dayjs().year() - 2 + i }))} />
        <span>年</span>
        <Select value={month} onChange={setMonth} style={{ width: 80 }} options={Array.from({ length: 12 }, (_, i) => ({ label: String(i + 1), value: i + 1 }))} />
        <span>月</span>
        <Button type="primary" onClick={loadData} loading={loading}>查询</Button>
        <Button onClick={handleExport}>导出Excel</Button>
      </Space>
      <Table columns={columns} dataSource={data} rowKey="lineNo" loading={loading} pagination={false} />
    </Card>
  );
};

export default CashFlow;
