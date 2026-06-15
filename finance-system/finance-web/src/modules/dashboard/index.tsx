// dashboard/index.tsx
import React, { useState, useEffect } from 'react';
import { Row, Col, Card, Statistic, Typography, Spin, Skeleton } from 'antd';
import {
  AccountBookOutlined,
  FileExcelOutlined,
  AuditOutlined,
  DollarOutlined,
  ClockCircleOutlined,
} from '@ant-design/icons';
import { useNavigate } from 'react-router-dom';
import { voucherApi } from '@/api/account';
import { approvalApi } from '@/api/approval';
import { expenseApi } from '@/api/expense';

const { Title } = Typography;

/** 工作台页面 */
const DashboardPage: React.FC = () => {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(true);
  const [stats, setStats] = useState({
    voucherCount: 0,
    pendingApproval: 0,
    monthlyExpense: 0,
    todoCount: 0,
  });

  useEffect(() => {
    loadStats();
  }, []);

  const loadStats = async () => {
    setLoading(true);
    try {
      // 并行请求各模块统计数据
      const [voucherRes, approvalRes, expenseRes] = await Promise.allSettled([
        voucherApi.page({ pageIndex: 1, pageSize: 1, currentMonth: true } as any),
        approvalApi.pending({ pageIndex: 1, pageSize: 1 } as any),
        expenseApi.statistics({ startDate: `${new Date().getFullYear()}-${String(new Date().getMonth() + 1).padStart(2, '0')}-01` }),
      ]);

      const voucherData = voucherRes.status === 'fulfilled' ? (voucherRes.value.data as any) : null;
      const approvalData = approvalRes.status === 'fulfilled' ? (approvalRes.value.data as any) : null;
      const expenseData = expenseRes.status === 'fulfilled' ? (expenseRes.value.data as any) : null;

      setStats({
        voucherCount: voucherData?.total || 0,
        pendingApproval: approvalData?.total || 0,
        monthlyExpense: expenseData ? (Array.isArray(expenseData) ? expenseData.reduce((s: number, d: any) => s + (d.totalAmount || 0), 0) : (expenseData.totalAmount || 0)) : 0,
        todoCount: voucherData?.total || 0,
      });
    } catch {
      // 数据加载失败时显示默认值
    } finally {
      setLoading(false);
    }
  };

  return (
    <div>
      <Title level={4}>工作台</Title>
      <Row gutter={[16, 16]} style={{ marginBottom: 24 }}>
        <Col span={6}>
          <Card>
            {loading ? <Skeleton active paragraph={{ rows: 1 }} /> : (
              <Statistic title="本月凭证数" value={stats.voucherCount} prefix={<FileExcelOutlined />} />
            )}
          </Card>
        </Col>
        <Col span={6}>
          <Card>
            {loading ? <Skeleton active paragraph={{ rows: 1 }} /> : (
              <Statistic title="待审批" value={stats.pendingApproval} prefix={<AuditOutlined />} valueStyle={{ color: '#faad14' }} />
            )}
          </Card>
        </Col>
        <Col span={6}>
          <Card>
            {loading ? <Skeleton active paragraph={{ rows: 1 }} /> : (
              <Statistic title="本月费用" value={stats.monthlyExpense} prefix={<DollarOutlined />} precision={2} />
            )}
          </Card>
        </Col>
        <Col span={6}>
          <Card>
            {loading ? <Skeleton active paragraph={{ rows: 1 }} /> : (
              <Statistic title="待处理" value={stats.todoCount} prefix={<ClockCircleOutlined />} valueStyle={{ color: '#ff4d4f' }} />
            )}
          </Card>
        </Col>
      </Row>
      <Row gutter={[16, 16]}>
        <Col span={12}>
          <Card title="快捷入口" size="small">
            <Row gutter={[8, 8]}>
              <Col span={8}><Card size="small" hoverable style={{ textAlign: 'center' }} onClick={() => navigate('/account/voucher')}><AccountBookOutlined /><div>凭证录入</div></Card></Col>
              <Col span={8}><Card size="small" hoverable style={{ textAlign: 'center' }} onClick={() => navigate('/report/balance-sheet')}><FileExcelOutlined /><div>报表查询</div></Card></Col>
              <Col span={8}><Card size="small" hoverable style={{ textAlign: 'center' }} onClick={() => navigate('/approval/pending')}><AuditOutlined /><div>审批中心</div></Card></Col>
              <Col span={8}><Card size="small" hoverable style={{ textAlign: 'center' }} onClick={() => navigate('/expense/claim')}><DollarOutlined /><div>费用报销</div></Card></Col>
              <Col span={8}><Card size="small" hoverable style={{ textAlign: 'center' }} onClick={() => navigate('/account/voucher')}><ClockCircleOutlined /><div>待办事项</div></Card></Col>
              <Col span={8}><Card size="small" hoverable style={{ textAlign: 'center' }} onClick={() => navigate('/system/user')}><AccountBookOutlined /><div>系统管理</div></Card></Col>
            </Row>
          </Card>
        </Col>
        <Col span={12}>
          <Card title="系统公告" size="small">
            <div style={{ padding: '8px 0' }}>暂无公告信息</div>
          </Card>
        </Col>
      </Row>
    </div>
  );
};

export default DashboardPage;
