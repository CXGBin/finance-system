import React, { useState } from 'react';
import { Card, Form, Input, DatePicker, Select, Table, Button, Space, Upload, Typography, message } from 'antd';
import { PlusOutlined, MinusCircleOutlined, UploadOutlined } from '@ant-design/icons';
import { useNavigate } from 'react-router-dom';
import { formatMoney } from '@/utils/format';

const { Title, Text } = Typography;

interface ClaimItem {
  key: string;
  expenseDate: string;
  description: string;
  amount: number;
  invoiceNo?: string;
}

const ExpenseClaimAdd: React.FC = () => {
  const navigate = useNavigate();
  const [items, setItems] = useState<ClaimItem[]>([
    { key: '1', expenseDate: '', description: '', amount: 0 },
  ]);

  const totalAmount = items.reduce((s, i) => s + (Number(i.amount) || 0), 0);

  const addItem = () => setItems((prev) => [...prev, { key: String(Date.now()), expenseDate: '', description: '', amount: 0 }]);
  const removeItem = (key: string) => setItems((prev) => prev.filter((i) => i.key !== key));
  const updateItem = (key: string, field: keyof ClaimItem, value: any) =>
    setItems((prev) => prev.map((i) => (i.key === key ? { ...i, [field]: value } : i)));

  const handleSubmit = () => {
    message.success('报销单已提交');
    navigate('/expense/claim');
  };

  const itemColumns = [
    { title: '费用日期', dataIndex: 'expenseDate', key: 'expenseDate', width: 140,
      render: (val: string, record: ClaimItem) => (
        <DatePicker style={{ width: '100%' }} onChange={(_, ds) => updateItem(record.key, 'expenseDate', ds || '')} />
      ),
    },
    { title: '费用说明', dataIndex: 'description', key: 'description', width: 200,
      render: (val: string, record: ClaimItem) => (
        <Input value={val} onChange={(e) => updateItem(record.key, 'description', e.target.value)} placeholder="说明" />
      ),
    },
    { title: '金额', dataIndex: 'amount', key: 'amount', width: 140,
      render: (val: number, record: ClaimItem) => (
        <Input type="number" value={val || ''} style={{ textAlign: 'right' }}
          onChange={(e) => updateItem(record.key, 'amount', parseFloat(e.target.value) || 0)} />
      ),
    },
    { title: '发票号', dataIndex: 'invoiceNo', key: 'invoiceNo', width: 160,
      render: (val: string, record: ClaimItem) => (
        <Input value={val} onChange={(e) => updateItem(record.key, 'invoiceNo', e.target.value)} placeholder="选填" />
      ),
    },
    {
      title: '操作', width: 60,
      render: (_: any, record: ClaimItem) =>
        items.length > 1 ? <MinusCircleOutlined onClick={() => removeItem(record.key)} style={{ color: '#ff4d4f', cursor: 'pointer' }} /> : null,
    },
  ];

  return (
    <div>
      <Title level={5}>提交报销</Title>
      <Card size="small">
        <Form layout="vertical" style={{ maxWidth: 600, marginBottom: 16 }}>
          <Form.Item label="报销标题" rules={[{ required: true }]}><Input /></Form.Item>
          <Form.Item label="费用类型"><Select placeholder="请选择"><Select.Option value="travel">差旅费</Select.Option><Select.Option value="office">办公费</Select.Option><Select.Option value="meal">餐饮费</Select.Option></Select></Form.Item>
        </Form>

        <Text strong style={{ marginBottom: 8, display: 'block' }}>费用明细</Text>
        <Table columns={itemColumns} dataSource={items} rowKey="key" pagination={false} size="small"
          footer={() => (
            <div style={{ display: 'flex', justifyContent: 'space-between' }}>
              <a onClick={addItem}><PlusOutlined /> 增加行</a>
              <Text>合计：<b>{formatMoney(totalAmount)}</b> 元</Text>
            </div>
          )}
        />

        <div style={{ marginTop: 16, marginBottom: 16 }}>
          <Upload><Button icon={<UploadOutlined />}>上传附件</Button></Upload>
        </div>

        <Space>
          <Button onClick={() => navigate('/expense/claim')}>取消</Button>
          <Button type="primary" onClick={handleSubmit}>提交</Button>
        </Space>
      </Card>
    </div>
  );
};

export default ExpenseClaimAdd;
