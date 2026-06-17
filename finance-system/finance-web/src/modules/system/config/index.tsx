import React, { useState } from 'react';
import { Card, Form, Input, Button, message, Space, Empty, Spin, Alert } from 'antd';
import { SaveOutlined, ReloadOutlined } from '@ant-design/icons';
import type { SysConfig } from '@/types/system.d';
import { configApi } from '@/api/system';

/** 系统配置页面 */
const ConfigList: React.FC = () => {
  const [configs, setConfigs] = useState<SysConfig[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [form] = Form.useForm();

  const loadConfigs = async () => {
    setLoading(true);
    setError(null);
    try {
      const data = await configApi.list();
      setConfigs(data.data ?? []);
      const formValues: Record<string, string> = {};
      (data.data ?? []).forEach((item: SysConfig) => {
        formValues[item.configKey] = item.configValue;
      });
      form.setFieldsValue(formValues);
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : '加载配置数据失败');
    } finally {
      setLoading(false);
    }
  };

  React.useEffect(() => { loadConfigs(); }, []);

  const handleSave = async () => {
    const values = form.getFieldsValue();
    const items = configs.map((c) => ({ configKey: c.configKey, configValue: values[c.configKey] ?? '' }));
    await configApi.batchUpdate(items);
    message.success('保存成功');
    loadConfigs();
  };

  return (
    <Card title="系统配置" extra={
      <Space>
        <Button icon={<ReloadOutlined />} onClick={loadConfigs}>刷新</Button>
        <Button type="primary" icon={<SaveOutlined />} onClick={handleSave}>保存配置</Button>
      </Space>
    }>
      <Spin spinning={loading}>
        {error ? (
          <Alert type="error" message={error} showIcon action={<Button size="small" onClick={loadConfigs}>重试</Button>} />
        ) : configs.length > 0 ? (
          <Form form={form} layout="vertical">
            {configs.map((config) => (
              <Form.Item key={config.configKey} name={config.configKey} label={config.configName}>
                <Input />
              </Form.Item>
            ))}
          </Form>
        ) : !loading ? (
          <Empty description="暂无配置数据" />
        ) : null}
      </Spin>
    </Card>
  );
};

export default ConfigList;
