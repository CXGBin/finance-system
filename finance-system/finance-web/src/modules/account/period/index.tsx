import React, { useState, useEffect } from 'react';
import { Card, Table, Button, Tag, message, Popconfirm, Space, Select, InputNumber, Modal, Statistic, Row, Col } from 'antd';
import { periodApi, balanceApi } from '@/api/account';
import dayjs from 'dayjs';
import type { AccountingPeriod } from '@/types/account.d';

/** 会计期间管理 */
const PeriodList: React.FC = () => {
  const [year, setYear] = useState(dayjs().year());
  const [data, setData] = useState<AccountingPeriod[]>([]);
  const [loading, setLoading] = useState(false);
  const [balanceModalVisible, setBalanceModalVisible] = useState(false);
  const [balanceData, setBalanceData] = useState<{ isBalanced: boolean; debitTotal: number; creditTotal: number } | null>(null);
  const [balanceLoading, setBalanceLoading] = useState(false);
  const [selectedPeriodId, setSelectedPeriodId] = useState<number | undefined>(undefined);

  useEffect(() => { loadData(); }, [year]);

  const loadData = async () => {
    setLoading(true);
    try {
      const res = await periodApi.list(year);
      setData(res.data || []);
    } finally { setLoading(false); }
  };

  const handleClose = async (record: AccountingPeriod) => {
    await periodApi.close(record.id);
    message.success('结账成功');
    loadData();
  };

  const handleReopen = async (record: AccountingPeriod) => {
    await periodApi.unclose(record.id);
    message.success('反结账成功');
    loadData();
  };

  const handleCarryForward = async (record: AccountingPeriod) => {
    await periodApi.profitTransfer(record.id);
    message.success('损益结转成功');
    loadData();
  };

  /** 试算平衡检查 */
  const handleTrialBalance = async (periodId: number) => {
    setSelectedPeriodId(periodId);
    setBalanceModalVisible(true);
    setBalanceLoading(true);
    try {
      const res = await balanceApi.trialBalance(periodId);
      setBalanceData(res.data || null);
    } catch {
      message.error('试算平衡检查失败');
      setBalanceData(null);
    } finally {
      setBalanceLoading(false);
    }
  };

  const columns = [
    { title: '年度', dataIndex: 'periodYear', key: 'periodYear' },
    { title: '月份', dataIndex: 'periodMonth', key: 'periodMonth' },
    { title: '开始日期', dataIndex: 'beginDate', key: 'beginDate' },
    { title: '结束日期', dataIndex: 'endDate', key: 'endDate' },
    {
      title: '状态', dataIndex: 'isClosed', key: 'isClosed',
      render: (val: boolean) => val ? <Tag color="success">已结账</Tag> : <Tag color="processing">未结账</Tag>,
    },
    { title: '结账时间', dataIndex: 'closedTime', key: 'closedTime' },
    {
      title: '操作', key: 'action',
      render: (_: unknown, record: AccountingPeriod) => (
        <Space>
          <a onClick={() => handleTrialBalance(record.id as number)}>试算平衡</a>
          {record.isClosed === 0 && (
            <>
              <Popconfirm title="确认结账?" onConfirm={() => handleClose(record as AccountingPeriod)}><a>结账</a></Popconfirm>
              <Popconfirm title="确认期末结转?" onConfirm={() => handleCarryForward(record as AccountingPeriod)}><a>结转</a></Popconfirm>
            </>
          )}
          {record.isClosed === 1 && (
            <Popconfirm title="确认反结账?" onConfirm={() => handleReopen(record as AccountingPeriod)}><a>反结账</a></Popconfirm>
          )}
        </Space>
      ),
    },
  ];

  return (
    <Card title="会计期间">
      <Space style={{ marginBottom: 16 }}>
        <span>年度：</span>
        <InputNumber value={year} min={2020} max={2099} onChange={(v) => v && setYear(v)} />
      </Space>
      <Table columns={columns} dataSource={data} rowKey="id" loading={loading} pagination={false} />
      <Modal
        title="试算平衡检查"
        open={balanceModalVisible}
        onCancel={() => setBalanceModalVisible(false)}
        footer={<Button onClick={() => setBalanceModalVisible(false)}>关闭</Button>}
      >
        {balanceLoading && <div style={{ textAlign: 'center', padding: 20 }}>加载中...</div>}
        {balanceData && (
          <div>
            <Row gutter={16} style={{ marginBottom: 16 }}>
              <Col span={8}>
                <Statistic title="借方合计" value={balanceData.debitTotal} precision={2} />
              </Col>
              <Col span={8}>
                <Statistic title="贷方合计" value={balanceData.creditTotal} precision={2} />
              </Col>
              <Col span={8}>
                <Statistic
                  title="差额"
                  value={Math.abs(balanceData.debitTotal - balanceData.creditTotal)}
                  precision={2}
                  valueStyle={{ color: balanceData.isBalanced ? '#3f8600' : '#cf1322' }}
                />
              </Col>
            </Row>
            <div style={{ textAlign: 'center', fontSize: 16, fontWeight: 'bold', color: balanceData.isBalanced ? '#3f8600' : '#cf1322' }}>
              {balanceData.isBalanced ? '✓ 试算平衡' : '✗ 试算不平衡，借贷差额：' + (balanceData.debitTotal - balanceData.creditTotal).toFixed(2)}
            </div>
          </div>
        )}
      </Modal>
    </Card>
  );
};

export default PeriodList;
