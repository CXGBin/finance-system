import React, { useState } from 'react';
import { Card, Form, Input, Button, message, Space, Empty } from 'antd';
import { SaveOutlined, ReloadOutlined } from '@ant-design/icons';
import type { SysConfig } from '@/types/system.d';
import { configApi } from '@/api/system';

/** 系统配置页面 */
const ConfigList: React.FC = () => {
  const [configs, setConfigs] = useState<SysConfig[]>([]);
  const [loading, setLoading] = useState(false);
  const [form] = Form.useForm();

  const loadConfigs = async () => {
    setLoading(true);
    try {
      const data = await configApi.list();
      setConfigs(data.data ?? []);
      // 设置表单值
      const formValues: Record<string, string> = {};
      (data.data ?? []).forEach((item: SysConfig) => {
        formValues[item.configKey] = item.configValue;
      });
      form.setFieldsValue(formValues);
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
    <Card title="系统配置" loading={loading} extra={
      <Space>
        <Button icon={<ReloadOutlined />} onClick={loadConfigs}>刷新</Button>
        <Button type="primary" icon={<SaveOutlined />} onClick={handleSave}>保存配置</Button>
      </Space>
    }>
      {configs.length > 0 ? (
        <Form form={form} layout="vertical">
          {configs.map((config) => (
            <Form.Item key={config.configKey} name={config.configKey} label={config.configName}>
              <Input />
            </Form.Item>
          ))}
        </Form>
      ) : (
        <Empty description="暂无配置数据" />
      )}
    </Card>
  );
};

export default ConfigList;
