import SearchBar from "@components/Ui/SearchBar";
import { Pagination, Select } from "@mantine/core";
import queryStringGenerator from "@utils/queryStringGenerator";
import React, { useEffect, useState } from "react";
import { useSearchParams } from "react-router-dom";

export interface IWithSearchPagination {
  searchParams: string;
  pagination: (totalPage: number) => JSX.Element;
  searchComponent: (placeholder?: string) => JSX.Element;
  sortComponent: (
    data: {
      value: string;
      label: string;
    }[],
    placeholder: string
  ) => JSX.Element;
  filterComponent: (
    data: {
      value: string;
      label: string;
    }[],
    placeholder: string,
    key: string
  ) => JSX.Element;
  setInitialSearch: React.Dispatch<
    React.SetStateAction<
      {
        key: string;
        value: string;
      }[]
    >
  >;
}

const withSearchPagination =
  <P extends object>(Component: React.FC<P & IWithSearchPagination>) =>
  (props: P) => {
    const [params, setParams] = useSearchParams();
    const [initialSearch, setInitialSearch] = useState<
      {
        key: string;
        value: string;
      }[]
    >([
      {
        key: "",
        value: "",
      },
    ]);

    let page = parseInt(params.get("page") ?? "1");
    let search = params.get("s") ?? null;
    let size = 12;
    const [searchParams, setSearchParams] = useState("");
    const [sortValue, setSortValue] = useState<string>(":");
    const [filterKey, setFilterKey] = useState<string>("");
    const [filterValue, setFilterValue] = useState<string>("");

    const setSearch = (search: string) => {
      for (let value of params.entries()) {
        if (value[0] !== "s") params.delete(value[0]);
      }
      params.set("s", search);
      setParams(params);
    };
    const sortComponent = (
      data: { value: string; label: string }[],
      placeholder: string
    ) => {
      return (
        <Select
          placeholder={placeholder}
          ml={5}
          clearable
          data={data}
          onChange={(e: string) => {
            setSortValue(e);
          }}
        />
      );
    };

    const filterComponent = (
      data: { value: string; label: string }[],
      placeholder: string,
      key: string
    ) => {
      return (
        <Select
          placeholder={placeholder}
          ml={5}
          clearable
          value={filterValue}
          data={data}
          onChange={(e: string) => {
            setFilterValue(() => e);
            if (e) {
              setFilterKey(() => key);
            } else {
              setFilterKey(() => "");
            }
          }}
        />
      );
    };

    const setPage = (pageNumber: number) => {
      params.set("page", pageNumber.toString());
      setParams(params);
    };

    useEffect(() => {
      const sortBy = (sortValue && sortValue.split(":")[0]) ?? "";
      const sortType = (sortValue && sortValue.split(":")[1]) ?? "";

      const data = {
        page,
        search: search ?? "",
        size,
        sortBy,
        sortType,
        [filterKey]: filterValue,
      };

      initialSearch.forEach((d) => {
        data[d.key] = d.value;
      });
      setSearchParams(queryStringGenerator(data));
    }, [page, search, size, sortValue, filterValue]);

    const pagination = (totalPage: number) =>
      totalPage > 0 ? (
        <Pagination my={20} total={totalPage} page={page} onChange={setPage} />
      ) : (
        <></>
      );
    const searchComponent = (placeholder?: string) => (
      <SearchBar
        search={search ?? ""}
        setSearch={setSearch}
        placeholder={placeholder}
      />
    );

    return (
      <Component
        {...(props as P)}
        filterComponent={filterComponent}
        searchParams={searchParams}
        pagination={pagination}
        searchComponent={searchComponent}
        sortComponent={sortComponent}
        setInitialSearch={setInitialSearch}
      />
    );
  };

export default withSearchPagination;
