import React, { useState, useEffect, useRef } from 'react';
import { Card, Table, Space, Button, Tag, Modal, Form, Input, message, Popconfirm } from 'antd';
import { PlusOutlined, EditOutlined, DeleteOutlined, ReloadOutlined } from '@ant-design/icons';
import { ProTable, type ProColumns, type ActionType } from '@ant-design/pro-components';
import { reportApi } from '@/api/report';

interface ReportTemplate { id: number; templateName: string; description: string; templateData: string; createdTime: string; }

/** 自定义报表 */
const CustomReport: React.FC = () => {
  const actionRef = useRef<ActionType>();
  const [modalOpen, setModalOpen] = useState(false);
  const [editingRecord, setEditingRecord] = useState<ReportTemplate | null>(null);
  const [form] = Form.useForm();

  const columns: ProColumns<ReportTemplate>[] = [
    { title: '报表名称', dataIndex: 'templateName', ellipsis: true },
    { title: '描述', dataIndex: 'description', search: false, ellipsis: true },
    { title: '创建时间', dataIndex: 'createdTime', valueType: 'dateTime', sorter: true, search: false, width: 180 },
    {
      title: '操作', valueType: 'option', width: 160,
      render: (_, record) => (
        <Space>
          <a>预览</a>
          <a onClick={() => { setEditingRecord(record); form.setFieldsValue(record); setModalOpen(true); }}>编辑</a>
          <Popconfirm title="确认删除?" onConfirm={() => { message.success('删除成功'); actionRef.current?.reload(); }}><a style={{ color: '#ff4d4f' }}>删除</a></Popconfirm>
        </Space>
      ),
    },
  ];

  return (
    <>
      <Card title="自定义报表">
        <ProTable<ReportTemplate>
          actionRef={actionRef} headerTitle="" rowKey="id" columns={columns}
          search={false}
          request={async () => {
            const res = await reportApi.customList();
            const list = res.data ?? [];
            return { data: list, success: true, total: list.length };
          }}
          toolBarRender={() => [<Button key="add" type="primary" icon={<PlusOutlined />} onClick={() => { setEditingRecord(null); form.resetFields(); setModalOpen(true); }}>新建报表</Button>]}
          pagination={false}
        />
      </Card>
      <Modal title={editingRecord ? '编辑报表' : '新建报表'} open={modalOpen} onOk={async () => {
        try {
          const values = await form.validateFields();
          if (editingRecord) { await reportApi.customUpdate(editingRecord.id, values); message.success('更新成功'); }
          else { await reportApi.customCreate(values); message.success('创建成功'); }
          setModalOpen(false); actionRef.current?.reload();
        } catch { /* validation */ }
      }} onCancel={() => setModalOpen(false)} width={600}>
        <Form form={form} layout="vertical">
          <Form.Item name="templateName" label="报表名称" rules={[{ required: true }]}><Input /></Form.Item>
          <Form.Item name="description" label="描述"><Input.TextArea rows={3} /></Form.Item>
          <Form.Item name="templateData" label="报表配置(JSON)"><Input.TextArea rows={6} /></Form.Item>
        </Form>
      </Modal>
    </>
  );
};

export default CustomReport;
