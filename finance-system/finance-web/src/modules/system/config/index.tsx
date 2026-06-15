import React, { useState, useEffect } from 'react';
import { Form, Input, Button, message, Card, Row, Col, Divider } from 'antd';
import type { SysConfig } from '@/types/system.d';
import { configApi } from '@/api/system';

/** 系统配置页面 */
const ConfigList: React.FC = () => {
  const [configs, setConfigs] = useState<SysConfig[]>([]);
  const [form] = Form.useForm();
  const [loading, setLoading] = useState(false);

  useEffect(() => { loadConfigs(); }, []);

  const loadConfigs = async () => {
    const res = await configApi.list();
    const data = res.data || [];
    setConfigs(data);
    const formValues: Record<string, string> = {};
    data.forEach((c: SysConfig) => { formValues[c.key || c.configKey || ''] = c.configValue; });
    form.setFieldsValue(formValues);
  };

  const handleSave = async () => {
    setLoading(true);
    try {
      const values = form.getFieldsValue();
      for (const [key, value] of Object.entries(values)) {
        await configApi.update({ configKey: key, configValue: String(value) } as any);
      }
      message.success('保存成功');
    } catch {} finally { setLoading(false); }
  };

  return (
    <Card title="系统配置">
      <Form form={form} layout="vertical">
        <Row gutter={24}>
          {configs.map((config: SysConfig, index: number) => (
            <Col span={8} key={config.id || index}>
              <Form.Item name={config.key || config.configKey} label={config.configName || config.configKey}>
                <Input placeholder={`请输入${config.configName || config.configKey}`} />
              </Form.Item>
            </Col>
          ))}
        </Row>
        <Divider />
        <Button type="primary" onClick={handleSave} loading={loading}>保存配置</Button>
      </Form>
    </Card>
  );
};

export default ConfigList;
