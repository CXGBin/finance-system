import React, { useState, useEffect } from 'react';
import { Card, Table, Tag, Button, Space, InputNumber, Select, message, Popconfirm, Modal, Statistic, Row, Col } from 'antd';
import { PlusOutlined, CheckCircleOutlined, CloseCircleOutlined, SyncOutlined, CarryForwardOutlined } from '@ant-design/icons';
import { periodApi } from '@/api/account';
import type { AccountingPeriod } from '@/types/account.d';

/** 会计期间管理页面 */
const PeriodList: React.FC = () => {
  const [year, setYear] = useState(new Date().getFullYear());
  const [periods, setPeriods] = useState<AccountingPeriod[]>([]);
  const [loading, setLoading] = useState(false);
  const [initModalOpen, setInitModalOpen] = useState(false);
  const [initYear, setInitYear] = useState(year + 1);

  const loadPeriods = async () => {
    setLoading(true);
    try {
      const res = await periodApi.list(year);
      setPeriods(res.data || []);
    } finally { setLoading(false); }
  };

  useEffect(() => { loadPeriods(); }, [year]);

  const handleClose = async (id: number) => {
    await periodApi.close(id);
    message.success('期末结账成功');
    loadPeriods();
  };

  const handleUnclose = async (id: number) => {
    await periodApi.unclose(id);
    message.success('反结账成功');
    loadPeriods();
  };

  const handleProfitTransfer = async (id: number) => {
    await periodApi.profitTransfer(id);
    message.success('损益结转成功');
    loadPeriods();
  };

  const handleInitYear = async () => {
    await periodApi.initYear(initYear);
    message.success(`${initYear}年度初始化成功`);
    setInitModalOpen(false);
    setYear(initYear);
  };

  const statusTag = (status: number) => {
    const map: Record<number, { color: string; text: string; icon: React.ReactNode }> = {
      0: { color: 'default', text: '未结账', icon: <CloseCircleOutlined /> },
      1: { color: 'success', text: '已结账', icon: <CheckCircleOutlined /> },
    };
    const info = map[status] || map[0];
    return <Tag color={info.color} icon={info.icon}>{info.text}</Tag>;
  };

  const columns = [
    { title: '年度', dataIndex: 'periodYear', key: 'periodYear', width: 80 },
    { title: '月份', dataIndex: 'periodMonth', key: 'periodMonth', width: 80, sorter: true },
    {
      title: '状态', dataIndex: 'status', key: 'status', width: 100,
      filters: [{ text: '未结账', value: 0 }, { text: '已结账', value: 1 }],
      render: (val: number) => statusTag(val),
    },
    { title: '开始日期', dataIndex: 'startDate', key: 'startDate', search: false },
    { title: '结束日期', dataIndex: 'endDate', key: 'endDate', search: false },
    {
      title: '操作', key: 'action', width: 200,
      render: (_: unknown, record: AccountingPeriod) => (
        <Space>
          {record.status === 0 && (
            <>
              <Popconfirm title="确认损益结转?" onConfirm={() => handleProfitTransfer(record.id)}>
                <a><CarryForwardOutlined /> 损益结转</a>
              </Popconfirm>
              <Popconfirm title="确认结账?" onConfirm={() => handleClose(record.id)}>
                <a style={{ color: '#1890ff' }}>结账</a>
              </Popconfirm>
            </>
          )}
          {record.status === 1 && (
            <Popconfirm title="确认反结账?" onConfirm={() => handleUnclose(record.id)}>
              <a style={{ color: '#faad14' }}><SyncOutlined /> 反结账</a>
            </Popconfirm>
          )}
        </Space>
      ),
    },
  ];

  const closedCount = periods.filter(p => p.status === 1).length;

  return (
    <>
      <Card title="会计期间管理" extra={
        <Space>
          <InputNumber value={year} min={2000} max={2100} onChange={(v) => v && setYear(v)} />
          <Button onClick={loadPeriods}>刷新</Button>
          <Button type="primary" icon={<PlusOutlined />} onClick={() => setInitModalOpen(true)}>初始化年度</Button>
        </Space>
      }>
        <Row gutter={16} style={{ marginBottom: 16 }}>
          <Col span={8}><Statistic title={`${year}年总期间`} value={periods.length} /></Col>
          <Col span={8}><Statistic title="已结账" value={closedCount} valueStyle={{ color: '#3f8600' }} /></Col>
          <Col span={8}><Statistic title="未结账" value={periods.length - closedCount} valueStyle={{ color: '#cf1322' }} /></Col>
        </Row>
        <Table columns={columns} dataSource={periods} rowKey="id" loading={loading} pagination={false} />
      </Card>
      <Modal title="初始化年度" open={initModalOpen} onOk={handleInitYear} onCancel={() => setInitModalOpen(false)}>
        <p>将创建选定年度的12个会计期间</p>
        <InputNumber value={initYear} min={2000} max={2100} onChange={(v) => v && setInitYear(v)} style={{ width: 120 }} />
      </Modal>
    </>
  );
};

export default PeriodList;
