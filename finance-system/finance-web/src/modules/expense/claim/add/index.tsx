// expense/claim/add/index.tsx
import React, { useState } from 'react';
import { Card, Form, Input, DatePicker, Select, Table, Button, Space, Upload, Typography, message } from 'antd';
import { PlusOutlined, MinusCircleOutlined, UploadOutlined } from '@ant-design/icons';
import { useNavigate } from 'react-router-dom';
import { expenseApi } from '@/api/expense';

const { Title, Text } = Typography;

interface ClaimItem {
  key: string;
  expenseDate: string;
  expenseTypeName: string;
  expenseTypeId: number;
  description: string;
  amount: number;
  invoiceNo?: string;
}

/** 提交报销页面 */
const ExpenseClaimAdd: React.FC = () => {
  const navigate = useNavigate();
  const [form] = Form.useForm();
  const [items, setItems] = useState<ClaimItem[]>([
    { key: '1', expenseDate: '', expenseTypeName: '', expenseTypeId: 0, description: '', amount: 0 },
  ]);
  const [expenseTypes, setExpenseTypes] = useState<any[]>([]);
  const [submitting, setSubmitting] = useState(false);

  const totalAmount = items.reduce((s, i) => s + (Number(i.amount) || 0), 0);

  /** 加载费用类型列表 */
  React.useEffect(() => {
    expenseApi.typeList().then(res => setExpenseTypes(res.data || []));
  }, []);

  const addItem = () => setItems((prev) => [...prev, { key: String(Date.now()), expenseDate: '', expenseTypeName: '', expenseTypeId: 0, description: '', amount: 0 }]);
  const removeItem = (key: string) => setItems((prev) => prev.filter((i) => i.key !== key));
  const updateItem = (key: string, field: keyof ClaimItem, value: string | number) =>
    setItems((prev) => prev.map((i) => (i.key === key ? { ...i, [field]: value } : i)));

  const handleSubmit = async () => {
    try {
      const formValues = await form.validateFields();
      if (items.length === 0 || !items.some(i => i.amount > 0)) {
        message.warning('请至少添加一条有效的费用明细');
        return;
      }
      setSubmitting(true);
      await expenseApi.claimAdd({
        title: formValues.title,
        expenseTypeId: formValues.expenseTypeId,
        items: items.map(item => ({
          expenseDate: item.expenseDate,
          expenseTypeId: item.expenseTypeId,
          expenseTypeName: item.expenseTypeName,
          description: item.description,
          amount: Number(item.amount),
          invoiceNo: item.invoiceNo,
        })),
      } as any);
      message.success('报销单已提交');
      navigate('/expense/claim');
    } catch (err) {
      console.error('提交报销失败:', err);
    } finally {
      setSubmitting(false);
    }
  };

  const itemColumns = [
    { title: '费用日期', dataIndex: 'expenseDate', key: 'expenseDate', width: 140,
      render: (val: string, record: ClaimItem) => (
        <DatePicker style={{ width: '100%' }} onChange={(_, ds) => updateItem(record.key, 'expenseDate', ds || '')} />
      ),
    },
    { title: '费用类型', dataIndex: 'expenseTypeName', key: 'expenseTypeName', width: 140,
      render: (val: string, record: ClaimItem) => (
        <Select
          style={{ width: '100%' }}
          placeholder="选择类型"
          value={record.expenseTypeId || undefined}
          onChange={(v, opt) => { updateItem(record.key, 'expenseTypeId', v); updateItem(record.key, 'expenseTypeName', (opt as any)?.label || ''); }}
          options={expenseTypes.map(t => ({ label: t.typeName, value: t.id }))}
        />
      ),
    },
    { title: '费用说明', dataIndex: 'description', key: 'description', width: 200,
      render: (val: string, record: ClaimItem) => (
        <Input value={val} onChange={(e) => updateItem(record.key, 'description', e.target.value)} placeholder="说明" />
      ),
    },
    { title: '金额', dataIndex: 'amount', key: 'amount', width: 120,
      render: (val: number, record: ClaimItem) => (
        <Input type="number" value={val || ''} style={{ textAlign: 'right' }}
          onChange={(e) => updateItem(record.key, 'amount', parseFloat(e.target.value) || 0)} />
      ),
    },
    { title: '发票号', dataIndex: 'invoiceNo', key: 'invoiceNo', width: 140,
      render: (val: string, record: ClaimItem) => (
        <Input value={val} onChange={(e) => updateItem(record.key, 'invoiceNo', e.target.value)} placeholder="选填" />
      ),
    },
    {
      title: '操作', width: 50,
      render: (_: unknown, record: ClaimItem) =>
        items.length > 1 ? <MinusCircleOutlined onClick={() => removeItem(record.key)} style={{ color: '#ff4d4f', cursor: 'pointer' }} /> : null,
    },
  ];

  return (
    <div>
      <Title level={5}>提交报销</Title>
      <Card size="small">
        <Form form={form} layout="vertical" style={{ maxWidth: 600, marginBottom: 16 }}>
          <Form.Item name="title" label="报销标题" rules={[{ required: true, message: '请输入报销标题' }]}>
            <Input />
          </Form.Item>
          <Form.Item name="expenseTypeId" label="费用类型" rules={[{ required: true, message: '请选择费用类型' }]}>
            <Select placeholder="请选择费用类型" options={expenseTypes.map(t => ({ label: t.typeName, value: t.id }))} />
          </Form.Item>
        </Form>

        <Text strong style={{ marginBottom: 8, display: 'block' }}>费用明细</Text>
        <Table columns={itemColumns} dataSource={items} rowKey="key" pagination={false} size="small"
          footer={() => (
            <div style={{ display: 'flex', justifyContent: 'space-between' }}>
              <a onClick={addItem}><PlusOutlined /> 增加行</a>
              <Text>合计：<b>{totalAmount.toFixed(2)}</b> 元</Text>
            </div>
          )}
        />

        <div style={{ marginTop: 16, marginBottom: 16 }}>
          <Upload><Button icon={<UploadOutlined />}>上传附件</Button></Upload>
        </div>

        <Space>
          <Button onClick={() => navigate('/expense/claim')}>取消</Button>
          <Button type="primary" onClick={handleSubmit} loading={submitting}>提交</Button>
        </Space>
      </Card>
    </div>
  );
};

export default ExpenseClaimAdd;
