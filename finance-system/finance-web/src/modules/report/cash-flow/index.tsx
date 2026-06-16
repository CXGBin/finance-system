import React, { useState } from 'react';
import { Card, Select, Space, Button, Table, message } from 'antd';
import { FileExcelOutlined, ReloadOutlined } from '@ant-design/icons';
import { reportApi, reportExportApi } from '@/api/report';
import dayjs from 'dayjs';

/** 现金流量表 */
const CashFlow: React.FC = () => {
  const [period, setPeriod] = useState(dayjs().format('YYYY-MM'));
  const [data, setData] = useState<any[]>([]);
  const [loading, setLoading] = useState(false);

  const loadData = async () => {
    setLoading(true);
    try { const res = await reportApi.cashFlow(period); setData(res.data?.items ?? res.data ?? []); } finally { setLoading(false); }
  };

  React.useEffect(() => { loadData(); }, [period]);

  const handleExport = async () => {
    try { await reportExportApi.exportExcel('cash-flow', period); message.success('导出成功'); } catch { /* error */ }
  };

  const columns = [
    { title: '行次', dataIndex: 'lineNo', width: 60, align: 'center' },
    { title: '项目', dataIndex: 'itemName', ellipsis: true },
    { title: '本月金额', dataIndex: 'currentAmount', align: 'right', render: (v: number) => v ? <span className="amount-right">{v.toFixed(2)}</span> : '-' },
    { title: '本年累计', dataIndex: 'yearAmount', align: 'right', render: (v: number) => v ? <span className="amount-right">{v.toFixed(2)}</span> : '-' },
  ];

  return (
    <Card title="现金流量表" extra={
      <Space>
        <Space size={4}><span>期间：</span>
          <Select value={period} onChange={setPeriod} style={{ width: 140 }}
            options={Array.from({ length: 12 }, (_, i) => ({ label: dayjs().subtract(11 - i, 'month').format('YYYY-MM'), value: dayjs().subtract(11 - i, 'month').format('YYYY-MM') }))} />
        </Space>
        <Button icon={<ReloadOutlined />} onClick={loadData}>刷新</Button>
        <Button icon={<FileExcelOutlined />} onClick={handleExport}>导出</Button>
      </Space>
    }>
      <Table columns={columns} dataSource={data} rowKey={(r) => `${r.lineNo}-${r.itemName}`} loading={loading} pagination={false} size="middle" bordered />
    </Card>
  );
};

export default CashFlow;
