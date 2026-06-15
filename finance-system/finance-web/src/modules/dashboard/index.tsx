import React from 'react';
import { Row, Col, Card, Statistic, Typography } from 'antd';
import {
  AccountBookOutlined,
  FileExcelOutlined,
  AuditOutlined,
  DollarOutlined,
  ClockCircleOutlined,
} from '@ant-design/icons';

const { Title } = Typography;

const DashboardPage: React.FC = () => {
  return (
    <div>
      <Title level={4}>工作台</Title>
      <Row gutter={[16, 16]} style={{ marginBottom: 24 }}>
        <Col span={6}>
          <Card>
            <Statistic title="本月凭证数" value={128} prefix={<FileExcelOutlined />} />
          </Card>
        </Col>
        <Col span={6}>
          <Card>
            <Statistic title="待审批" value={12} prefix={<AuditOutlined />} valueStyle={{ color: '#faad14' }} />
          </Card>
        </Col>
        <Col span={6}>
          <Card>
            <Statistic title="本月费用" value={56800} prefix={<DollarOutlined />} precision={2} />
          </Card>
        </Col>
        <Col span={6}>
          <Card>
            <Statistic title="待处理" value={8} prefix={<ClockCircleOutlined />} valueStyle={{ color: '#ff4d4f' }} />
          </Card>
        </Col>
      </Row>
      <Row gutter={[16, 16]}>
        <Col span={12}>
          <Card title="快捷入口" size="small">
            <Row gutter={[8, 8]}>
              <Col span={8}><Card size="small" hoverable style={{ textAlign: 'center' }}><AccountBookOutlined /><div>凭证录入</div></Card></Col>
              <Col span={8}><Card size="small" hoverable style={{ textAlign: 'center' }}><FileExcelOutlined /><div>报表查询</div></Card></Col>
              <Col span={8}><Card size="small" hoverable style={{ textAlign: 'center' }}><AuditOutlined /><div>审批中心</div></Card></Col>
              <Col span={8}><Card size="small" hoverable style={{ textAlign: 'center' }}><DollarOutlined /><div>费用报销</div></Card></Col>
              <Col span={8}><Card size="small" hoverable style={{ textAlign: 'center' }}><ClockCircleOutlined /><div>待办事项</div></Card></Col>
              <Col span={8}><Card size="small" hoverable style={{ textAlign: 'center' }}><AccountBookOutlined /><div>更多</div></Card></Col>
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
