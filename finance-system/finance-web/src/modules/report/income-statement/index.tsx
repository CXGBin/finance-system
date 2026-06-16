import React, { useState } from 'react';
import { Card, Table, Select, Space, Button, message } from 'antd';
import { reportApi, reportExportApi } from '@/api/report';
import type { IncomeStatementRow } from '@/types/report.d';
import dayjs from 'dayjs';

/** 利润表 */
const IncomeStatement: React.FC = () => {
  const [data, setData] = useState<IncomeStatementRow[]>([]);
  const [loading, setLoading] = useState(false);
  const [params, setParams] = useState({ year: dayjs().year(), month: dayjs().month() + 1 });

  const loadData = async () => {
    setLoading(true);
    try { const res = await reportApi.incomeStatement(params as any); setData(res.data || []); } finally { setLoading(false); }
  };

  /** 导出Excel */
  const handleExport = async () => {
    try {
      message.loading({ content: '正在导出...', key: 'export' });
      const res = await reportExportApi.excel({ reportType: 'income-statement', year: params.year, month: params.month });
      const link = document.createElement('a');
      link.href = res.data as string;
      link.download = `利润表_${params.year}_${params.month}.xlsx`;
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
    <Card title="
      <Space style={{ marginBottom: 16 }}>
        <Select value={params.year} onChange={(v) => setParams({ ...params, year: v })} style={{ width: 100 }} options={Array.from({ length: 5 }, (_, i) => ({ label: String(dayjs().year() - 2 + i), value: dayjs().year() - 2 + i }))} />
        <span>年</span>
        <Select value={params.month} onChange={(v) => setParams({ ...params, month: v })} style={{ width: 80 }} options={Array.from({ length: 12 }, (_, i) => ({ label: String(i + 1), value: i + 1 }))} />
        <span>月</span>
        <Button type="primary" onClick={loadData} loading={loading}>查询</Button>
        <Button onClick={handleExport}>导出Excel</Button>
      </Space>
      <Table columns={columns} dataSource={data} rowKey="lineNo" loading={loading} pagination={false} />
    </Card>
  );
};

export default IncomeStatement;
