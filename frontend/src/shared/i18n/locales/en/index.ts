import app from "./common/app.json";
import actions from "./common/actions.json";
import layout from "./common/layout.json";
import pagination from "./common/pagination.json";
import meta from "./common/meta.json";
import states from "./common/states.json";

import errorsCommon from "./errors/common.json";
import errorsValidation from "./errors/validation.json";

import agenciesList from "./agencies/list.json";
import agenciesDetails from "./agencies/details.json";
import agenciesCard from "./agencies/card.json";

import brokersCommon from "./brokers/common.json";
import brokersList from "./brokers/list.json";
import brokersCard from "./brokers/card.json";

import propertiesList from "./properties/list.json";
import propertiesFilters from "./properties/filters.json";
import propertiesCard from "./properties/card.json";

import leadsForm from "./leads/form.json";
import leadsList from "./leads/list.json";
import leadsTable from "./leads/table.json";

import blogList from "./blog/list.json";
import blogDetails from "./blog/details.json";

import home from "./home.json"
import notFound from "./notFound.json";
import navigation from "./navigation.json";

const en = {
  home,
  notFound,
  navigation,
  common: {
    app,
    actions,
    layout,
    pagination,
    states,
    meta,
  },
  errors: {
    common: errorsCommon,
    validation: errorsValidation,
  },
  agencies: {
    list: agenciesList,
    details: agenciesDetails,
    card: agenciesCard,
  },
  brokers: {
    common: brokersCommon,
    list: brokersList,
    card: brokersCard,
  },
  properties: {
    list: propertiesList,
    filters: propertiesFilters,
    card: propertiesCard,
  },
  leads: {
    form: leadsForm,
    list: leadsList,
    table: leadsTable,
  },
  blog: {
    list: blogList,
    details: blogDetails,
  },
};

export default en;
