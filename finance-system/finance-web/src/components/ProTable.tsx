import React, { useState, useCallback, useEffect } from 'react';
import { Table, Button, Space, Input, Row, Col, Form, Typography, Empty, Spin } from 'antd';
import type { TableProps, ColumnType } from 'antd';
import { ReloadOutlined, DownOutlined, UpOutlined, PlusOutlined } from '@ant-design/icons';
import type { PagedResult, PageParams, SearchParams } from '@/types/api.d';

export interface ProTableColumn<T = Record<string, unknown>> extends ColumnType<T> {
  search?: boolean;
  searchRender?: React.ReactNode;
}

export interface ProTableProps<T = Record<string, unknown>> {
  columns: ProTableColumn<T>[];
  fetchData: (params: PageParams & SearchParams) => Promise<PagedResult<T>>;
  rowKey?: string | ((record: T) => string);
  toolbarActions?: React.ReactNode;
  searchInitialValues?: SearchParams;
  tableProps?: Partial<TableProps<T>>;
  defaultPageSize?: number;
}

function ProTable<T extends Record<string, unknown>>({
  columns,
  fetchData,
  rowKey = 'id' as string | ((record: T) => string),
  toolbarActions,
  searchInitialValues = {},
  tableProps,
  defaultPageSize = 10,
}: ProTableProps<T>) {
  const [form] = Form.useForm();
  const [data, setData] = useState<T[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [total, setTotal] = useState(0);
  const [pageParams, setPageParams] = useState<PageParams>({ page: 1, pageSize: defaultPageSize });
  const [searchExpanded, setSearchExpanded] = useState(false);

  const loadData = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const values = form.getFieldsValue();
      const res = await fetchData({ ...pageParams, ...values });
      setData(res.data?.list || []);
      setTotal(res.data?.total || 0);
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : '加载失败');
    } finally {
      setLoading(false);
    }
  }, [fetchData, pageParams, form]);

  useEffect(() => {
    loadData();
  }, [loadData]);

  const handleSearch = () => {
    setPageParams((p) => ({ ...p, page: 1 }));
  };

  const handleReset = () => {
    form.resetFields();
    setPageParams((p) => ({ ...p, page: 1 }));
  };

  const handlePageChange = (page: number, pageSize: number) => {
    setPageParams({ page, pageSize });
  };

  const searchColumns = columns.filter((col) => col.search);
  const showSearch = searchColumns.length > 0;

  const visibleSearchCols = searchExpanded ? searchColumns : searchColumns.slice(0, 3);

  const renderContent = () => {
    if (error) return <Empty description={error} />;
    return (
      <Table<T>
        {...tableProps}
        rowKey={rowKey}
        columns={columns}
        dataSource={data}
        loading={loading}
        pagination={{
          current: pageParams.page,
          pageSize: pageParams.pageSize,
          total,
          showSizeChanger: true,
          showQuickJumper: true,
          showTotal: (t) => `共 ${t} 条`,
          onChange: handlePageChange,
        }}
        scroll={{ x: 'max-content' }}
      />
    );
  };

  return (
    <div>
      {showSearch && (
        <Form form={form} initialValues={searchInitialValues} className="pro-table-search">
          <Row gutter={[16, 16]}>
            {visibleSearchCols.map((col) => (
              <Col span={6} key={col.dataIndex as string}>
                <Form.Item name={col.dataIndex as string} label={col.title as string}>
                  {col.searchRender || <Input placeholder={`请输入${col.title}`} allowClear />}
                </Form.Item>
              </Col>
            ))}
            <Col span={searchExpanded ? 6 : 6} style={{ textAlign: 'right' }}>
              <Space>
                <Button type="primary" onClick={handleSearch}>
                  查询
                </Button>
                <Button onClick={handleReset}>重置</Button>
                {searchColumns.length > 3 && (
                  <Button
                    type="link"
                    onClick={() => setSearchExpanded(!searchExpanded)}
                    icon={searchExpanded ? <UpOutlined /> : <DownOutlined />}
                  >
                    {searchExpanded ? '收起' : '展开'}
                  </Button>
                )}
              </Space>
            </Col>
          </Row>
        </Form>
      )}

      <div className="pro-table-toolbar" style={{ marginBottom: 16 }}>
        <Space>
          <Button
            type="primary"
            icon={<ReloadOutlined />}
            onClick={loadData}
            loading={loading}
          >
            刷新
          </Button>
          {toolbarActions}
        </Space>
      </div>

      {renderContent()}
    </div>
  );
}

export default ProTable;
