import React, { useState, useRef } from 'react';
import { Card, Select, InputNumber, Button, message, Space, Modal, Form, Popconfirm, Tag } from 'antd';
import { ProTable, type ProColumns, type ActionType } from '@ant-design/pro-components';
import { PlusOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons';
import { budgetApi } from '@/api/budget';
import type { BudgetItem } from '@/types/budget.d';
import { createProTableRequest } from '@/utils/proTableRequest';

/** 预算年度设置 */
const BudgetSetting: React.FC = () => {
  const [years, setYears] = useState<{ id: number; year: number; status: number }[]>([]);
  const [selectedYearId, setSelectedYearId] = useState<number | undefined>();

  React.useEffect(() => {
    budgetApi.years().then(res => {
      const list = res.data ?? [];
      setYears(list);
      const active = list.find(y => y.status === 1) ?? list[0];
      if (active) setSelectedYearId(active.id);
    });
  }, []);

  const handleCreateYear = async () => {
    const newYear = years.length > 0 ? Math.max(...years.map(y => y.year)) + 1 : new Date().getFullYear() + 1;
    await budgetApi.createYear({ year: newYear });
    message.success(`${newYear}年度已创建`);
    budgetApi.years().then(res => { setYears(res.data ?? []); });
  };

  return (
    <Card title="预算年度设置" extra={
      <Space>
        <Select value={selectedYearId} onChange={setSelectedYearId} style={{ width: 140 }} placeholder="选择年度"
          options={years.map(y => ({ label: `${y.year}年${y.status === 1 ? ' (启用)' : ''}`, value: y.id }))} />
        <Button onClick={handleCreateYear}>新建年度</Button>
      </Space>
    }>
      {selectedYearId && <BudgetSubjectTable yearId={selectedYearId} />}
    </Card>
  );
};

/** 预算科目表格 */
const BudgetSubjectTable: React.FC<{ yearId: number }> = ({ yearId }) => {
  const actionRef = useRef<ActionType>();
  const [modalOpen, setModalOpen] = useState(false);
  const [editingRecord, setEditingRecord] = useState<BudgetItem | null>(null);
  const [form] = Form.useForm();

  const columns: ProColumns<BudgetItem>[] = [
    { title: '科目名称', dataIndex: 'subjectName', ellipsis: true },
    { title: '年度预算', dataIndex: 'annualAmount', sorter: true, search: false, align: 'right', render: (_, r) => <span className="amount-right">¥{(r.annualAmount ?? 0).toFixed(2)}</span> },
    { title: '部门', dataIndex: 'deptName', search: false, ellipsis: true },
    { title: '备注', dataIndex: 'remark', search: false, ellipsis: true },
    {
      title: '操作', valueType: 'option', width: 140,
      render: (_, record) => (
        <Space>
          <a onClick={() => { setEditingRecord(record); form.setFieldsValue(record); setModalOpen(true); }}>编辑</a>
          <Popconfirm title="确认删除?" onConfirm={() => { budgetApi.subjectRemove(record.id); message.success('删除成功'); actionRef.current?.reload(); }}>
            <a style={{ color: '#ff4d4f' }}>删除</a>
          </Popconfirm>
        </Space>
      ),
    },
  ];

  return (
    <>
      <ProTable<BudgetItem> actionRef={actionRef} headerTitle="预算科目" rowKey="id" columns={columns}
        search={{ labelWidth: 'auto' }}
        request={createProTableRequest((params) => budgetApi.subjectList({ yearId, ...params }))}
        toolBarRender={() => [<Button key="add" type="primary" icon={<PlusOutlined />} onClick={() => { setEditingRecord(null); form.resetFields(); form.setFieldsValue({ annualAmount: 0 }); setModalOpen(true); }}>新增科目</Button>]}
        pagination={{ defaultPageSize: 10, showSizeChanger: true }}
      />
      <Modal title={editingRecord ? '编辑预算科目' : '新增预算科目'} open={modalOpen} onOk={async () => {
        try {
          const values = await form.validateFields();
          if (editingRecord) { await budgetApi.subjectUpdate({ ...editingRecord, ...values }); message.success('更新成功'); }
          else { await budgetApi.subjectAdd({ ...values, yearId } as any); message.success('新增成功'); }
          setModalOpen(false); actionRef.current?.reload();
        } catch { /* validation */ }
      }} onCancel={() => setModalOpen(false)} width={500}>
        <Form form={form} layout="vertical">
          <Form.Item name="subjectId" label="会计科目" rules={[{ required: true }]}><InputNumber style={{ width: '100%' }} /></Form.Item>
          <Form.Item name="deptId" label="部门"><InputNumber style={{ width: '100%' }} /></Form.Item>
          <Form.Item name="annualAmount" label="年度预算金额" rules={[{ required: true }]}><InputNumber style={{ width: '100%' }} precision={2} /></Form.Item>
          <Form.Item name="remark" label="备注"><Input.TextArea rows={2} /></Form.Item>
        </Form>
      </Modal>
    </>
  );
};

export default BudgetSetting;
