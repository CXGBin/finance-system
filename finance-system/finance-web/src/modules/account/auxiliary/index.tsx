import React, { useState, useCallback, useEffect } from 'react';
import { Card, Table, Button, Space, Input, Modal, Form, message, Popconfirm } from 'antd';
import { useSearchParams } from 'react-router-dom';
import { auxiliaryApi } from '@/api/account';

/** 辅助核算类型配置 */
const TYPE_CONFIG: Record<string, { label: string; columns: Array<{ title: string; dataIndex: string; search?: boolean }> }> = {
  customer: {
    label: '客户',
    columns: [
      { title: '编码', dataIndex: 'code', search: true },
      { title: '客户名称', dataIndex: 'name', search: true },
      { title: '联系人', dataIndex: 'contact' },
      { title: '联系电话', dataIndex: 'phone' },
      { title: '地址', dataIndex: 'address' },
    ],
  },
  supplier: {
    label: '供应商',
    columns: [
      { title: '编码', dataIndex: 'code', search: true },
      { title: '供应商名称', dataIndex: 'name', search: true },
      { title: '联系人', dataIndex: 'contact' },
      { title: '联系电话', dataIndex: 'phone' },
      { title: '地址', dataIndex: 'address' },
    ],
  },
  project: {
    label: '项目',
    columns: [
      { title: '项目编码', dataIndex: 'code', search: true },
      { title: '项目名称', dataIndex: 'name', search: true },
      { title: '负责人', dataIndex: 'manager' },
      { title: '开始日期', dataIndex: 'startDate' },
      { title: '状态', dataIndex: 'status' },
    ],
  },
};

/** 辅助核算管理页面 */
const AuxiliaryPage: React.FC = () => {
  const [searchParams] = useSearchParams();
  const type = searchParams.get('type') || 'customer';
  const config = TYPE_CONFIG[type] || TYPE_CONFIG.customer;

  const [data, setData] = useState<Record<string, unknown>[]>([]);
  const [loading, setLoading] = useState(false);
  const [modalOpen, setModalOpen] = useState(false);
  const [editingRecord, setEditingRecord] = useState<Record<string, unknown> | null>(null);
  const [searchValues, setSearchValues] = useState<Record<string, string>>({});
  const [form] = Form.useForm();

  const loadData = useCallback(async () => {
    setLoading(true);
    try {
      const res = await auxiliaryApi.list(type);
      let list = res.data || [];
      // 前端过滤搜索
      if (searchValues.code) {
        list = list.filter((item) => String(item.code || '').includes(searchValues.code));
      }
      if (searchValues.name) {
        list = list.filter((item) => String(item.name || '').includes(searchValues.name));
      }
      setData(list);
    } catch {
      message.error('加载辅助核算数据失败');
    } finally {
      setLoading(false);
    }
  }, [type, searchValues]);

  useEffect(() => { loadData(); }, [loadData]);

  const handleAdd = () => {
    setEditingRecord(null);
    form.resetFields();
    setModalOpen(true);
  };

  const handleEdit = (record: Record<string, unknown>) => {
    setEditingRecord(record);
    form.setFieldsValue(record);
    setModalOpen(true);
  };

  const handleSave = async () => {
    try {
      const values = await form.validateFields();
      if (editingRecord) {
        await auxiliaryApi.update(type, Number(editingRecord.id), values);
        message.success('更新成功');
      } else {
        await auxiliaryApi.add(type, values);
        message.success('新增成功');
      }
      setModalOpen(false);
      loadData();
    } catch {}
  };

  const handleDelete = async (id: number) => {
    try {
      await auxiliaryApi.remove(type, id);
      message.success('删除成功');
      loadData();
    } catch {
      message.error('删除失败');
    }
  };

  const columns = [
    ...config.columns.map((col) => ({
      ...col,
      key: col.dataIndex,
      search: undefined,
    })),
    {
      title: '操作',
      key: 'action',
      width: 150,
      render: (_: unknown, record: Record<string, unknown>) => (
        <Space>
          <a onClick={() => handleEdit(record)}>编辑</a>
          <Popconfirm title="确认删除?" onConfirm={() => handleDelete(Number(record.id))}>
            <a style={{ color: '#ff4d4f' }}>删除</a>
          </Popconfirm>
        </Space>
      ),
    },
  ];

  return (
    <Card title={`${config.label}管理`} extra={<Button type="primary" onClick={handleAdd}>新增</Button>}>
      <Space style={{ marginBottom: 16 }}>
        <Input
          placeholder="编码"
          value={searchValues.code}
          onChange={(e) => setSearchValues({ ...searchValues, code: e.target.value })}
          allowClear
          style={{ width: 150 }}
        />
        <Input
          placeholder="名称"
          value={searchValues.name}
          onChange={(e) => setSearchValues({ ...searchValues, name: e.target.value })}
          allowClear
          style={{ width: 150 }}
        />
        <Button type="primary" onClick={loadData}>查询</Button>
      </Space>
      <Table
        columns={columns}
        dataSource={data}
        rowKey="id"
        loading={loading}
        pagination={{ showSizeChanger: true, showTotal: (t) => `共 ${t} 条` }}
      />
      <Modal
        title={editingRecord ? `编辑${config.label}` : `新增${config.label}`}
        open={modalOpen}
        onOk={handleSave}
        onCancel={() => setModalOpen(false)}
        width={600}
      >
        <Form form={form} layout="vertical">
          {config.columns.map((col) => (
            <Form.Item key={col.dataIndex} name={col.dataIndex} label={col.title} rules={[{ required: col.dataIndex === 'code' || col.dataIndex === 'name' }]}>
              <Input />
            </Form.Item>
          ))}
        </Form>
      </Modal>
    </Card>
  );
};

export default AuxiliaryPage;
