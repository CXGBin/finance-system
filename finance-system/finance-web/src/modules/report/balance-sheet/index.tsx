import React, { useState } from 'react';
import { Card, Select, Space, Button, Table, Switch, message, Row, Col } from 'antd';
import { FileExcelOutlined, ReloadOutlined } from '@ant-design/icons';
import { reportApi, reportExportApi } from '@/api/report';
import dayjs from 'dayjs';

/** 资产负债表 */
const BalanceSheet: React.FC = () => {
  const [period, setPeriod] = useState(dayjs().format('YYYY-MM'));
  const [showZero, setShowZero] = useState(false);
  const [data, setData] = useState<any[]>([]);
  const [loading, setLoading] = useState(false);

  const loadData = async () => {
    setLoading(true);
    try { const res = await reportApi.balanceSheet(period, showZero); setData(res.data?.items ?? res.data ?? []); } finally { setLoading(false); }
  };

  React.useEffect(() => { loadData(); }, [period, showZero]);

  const handleExport = async () => {
    try { await reportExportApi.exportExcel('balance-sheet', period); message.success('导出成功'); } catch { /* error */ }
  };

  const columns = [
    { title: '行次', dataIndex: 'lineNo', width: 60, align: 'center' },
    { title: '项目', dataIndex: 'itemName', ellipsis: true },
    { title: '行次', dataIndex: 'lineNo', width: 60, align: 'center' },
    { title: '期末余额', dataIndex: 'endBalance', align: 'right', render: (v: number) => v ? <span className="amount-right">{v.toFixed(2)}</span> : '-' },
    { title: '年初余额', dataIndex: 'beginBalance', align: 'right', render: (v: number) => v ? <span className="amount-right">{v.toFixed(2)}</span> : '-' },
  ];

  return (
    <Card title="资产负债表" extra={
      <Space>
        <Space size={4}>
          <span>会计期间：</span>
          <Select value={period} onChange={setPeriod} style={{ width: 140 }}
            options={Array.from({ length: 12 }, (_, i) => ({ label: dayjs().subtract(11 - i, 'month').format('YYYY-MM'), value: dayjs().subtract(11 - i, 'month').format('YYYY-MM') }))} />
        </Space>
        <Space size={4}>
          <span>显示零值：</span>
          <Switch size="small" checked={showZero} onChange={setShowZero} />
        </Space>
        <Button icon={<ReloadOutlined />} onClick={loadData}>刷新</Button>
        <Button icon={<FileExcelOutlined />} onClick={handleExport}>导出</Button>
      </Space>
    }>
      <Table columns={columns} dataSource={data} rowKey={(r) => `${r.lineNo}-${r.itemName}`} loading={loading} pagination={false} size="middle" bordered />
    </Card>
  );
};

export default BalanceSheet;
