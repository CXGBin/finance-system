import React, { useState, useEffect } from 'react';
import { Card, Form, InputNumber, Button, message, Space } from 'antd';
import { budgetApi } from '@/api/budget';

/** 预算设置页面 */
const BudgetSetting: React.FC = () => {
  const [form] = Form.useForm();
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    budgetApi.getSetting().then(res => form.setFieldsValue((res.data as any) || {}));
  }, []);

  const handleSave = async () => {
    setLoading(true);
    try {
      const values = form.getFieldsValue();
      await budgetApi.updateSetting(values);
      message.success('保存成功');
    } finally { setLoading(false); }
  };

  return (
    <Card title="预算设置">
      <Form form={form} layout="vertical" style={{ maxWidth: 500 }}>
        <Form.Item name="budgetYear" label="预算年度" rules={[{ required: true }]}>
          <InputNumber style={{ width: '100%' }} min={2020} max={2099} />
        </Form.Item>
        <Form.Item name="approvalThreshold" label="预算审批阈值（元）">
          <InputNumber style={{ width: '100%' }} min={0} />
        </Form.Item>
        <Form.Item name="alertThreshold" label="预警阈值（%）">
          <InputNumber style={{ width: '100%' }} min={0} max={100} />
        </Form.Item>
        <Form.Item>
          <Space><Button type="primary" onClick={handleSave} loading={loading}>保存</Button></Space>
        </Form.Item>
      </Form>
    </Card>
  );
};

export default BudgetSetting;
